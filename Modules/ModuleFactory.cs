﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MemorySharp.Internals;
using MemorySharp.Memory;

namespace MemorySharp.Modules
{
    /// <summary>
    ///     Class providing tools for manipulating modules and libraries.
    /// </summary>
    public class ModuleFactory : IFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModuleFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemoryManagement.MemorySharp" /> object.</param>
        internal ModuleFactory(MemoryBase memorySharp)
        {
            // Save the parameter
            MemorySharp = memorySharp;
            // Create a list containing all injected modules
            InternalInjectedModules = new List<InjectedModule>();
        }

        /// <summary>
        ///     The list containing all injected modules (writable).
        /// </summary>
        protected readonly List<InjectedModule> InternalInjectedModules;

        /// <summary>
        ///     The reference of the <see cref="MemoryManagement.MemorySharp" /> object.
        /// </summary>
        protected readonly MemoryBase MemorySharp;

        /// <summary>
        ///     A collection containing all injected modules.
        /// </summary>
        public IEnumerable<InjectedModule> InjectedModules => InternalInjectedModules.AsReadOnly();

        /// <summary>
        ///     Gets the main module for the remote process.
        /// </summary>
        public RemoteModule MainModule => FetchModule(MemorySharp.Process.MainModule);

        /// <summary>
        ///     Gets the modules that have been loaded in the remote process.
        /// </summary>
        public IEnumerable<RemoteModule> RemoteModules => NativeModules.Select(FetchModule);

        /// <summary>
        ///     Gets the native modules that have been loaded in the remote process.
        /// </summary>
        internal IEnumerable<ProcessModule> NativeModules => MemorySharp.Process.Modules.Cast<ProcessModule>();

        /// <summary>
        ///     Gets a pointer from the remote process.
        /// </summary>
        /// <param name="address">The address of the pointer.</param>
        /// <returns>A new instance of a <see cref="RemotePointer" /> class.</returns>
        public RemotePointer this[IntPtr address] => new RemotePointer(MemorySharp, address);

        /// <summary>
        ///     Gets the specified module in the remote process.
        /// </summary>
        /// <param name="moduleName">The name of module (not case sensitive).</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        public RemoteModule this[string moduleName] => FetchModule(moduleName);

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="module">The module to eject.</param>
        public void Eject(RemoteModule module)
        {
            // If the module is valid
            if (!module.IsValid) return;

            // Find if the module is an injected one
            var injected = InternalInjectedModules.FirstOrDefault(m => m.Equals(module));
            if (injected != null)
                InternalInjectedModules.Remove(injected);

            // Eject the module
            RemoteModule.InternalEject(MemorySharp, module);
        }

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="moduleName">The name of module to eject.</param>
        public void Eject(string moduleName)
        {
            // Fint the module to eject
            var module = RemoteModules.FirstOrDefault(m => m.Name == moduleName);
            // Eject the module is it's valid
            if (module != null)
                RemoteModule.InternalEject(MemorySharp, module);
        }

        /// <summary>
        ///     Fetches a module from the remote process.
        /// </summary>
        /// <param name="moduleName">
        ///     A module name (not case sensitive). If the file name extension is omitted, the default library
        ///     extension .dll is appended.
        /// </param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        protected RemoteModule FetchModule(string moduleName)
        {
            // Convert module name with lower chars
            moduleName = moduleName.ToLower();
            // Check if the module name has an extension
            if (!Path.HasExtension(moduleName))
                moduleName += ".dll";

            // Fetch and return the module
            return new RemoteModule(MemorySharp, NativeModules.First(m => m.ModuleName.ToLower() == moduleName));
        }

        /// <summary>
        ///     Fetches a module from the remote process.
        /// </summary>
        /// <param name="module">A module in the remote process.</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        private RemoteModule FetchModule(ProcessModule module)
        {
            return FetchModule(module.ModuleName);
        }

        /// <summary>
        ///     Injects the specified module into the address space of the remote process.
        /// </summary>
        /// <param name="path">
        ///     The path of the module. This can be either a library module (a .dll file) or an executable module
        ///     (an .exe file).
        /// </param>
        /// <param name="mustBeDisposed">The module will be ejected when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="InjectedModule" />class.</returns>
        public InjectedModule Inject(string path, bool mustBeDisposed = true)
        {
            // Injects the module
            var module = InjectedModule.InternalInject(MemorySharp, path);
            // Add the module in the list
            InternalInjectedModules.Add(module);
            // Return the module
            return module;
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="ModuleFactory" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            // Release all injected modules which must be disposed
            foreach (var injectedModule in InternalInjectedModules.Where(m => m.MustBeDisposed))
            {
                injectedModule.Dispose();
            }
            // Clean the cached functions related to this process
            foreach (var cachedFunction in RemoteModule.CachedFunctions.ToArray())
            {
                if (cachedFunction.Key.Item2 == MemorySharp.Handle)
                    RemoteModule.CachedFunctions.Remove(cachedFunction);
            }
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~ModuleFactory()
        {
            Dispose();
        }
    }
}