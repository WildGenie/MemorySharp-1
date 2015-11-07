using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Binarysharp.MemoryManagement.Common.Extensions;
using Binarysharp.MemoryManagement.Common.Helpers;
using Binarysharp.MemoryManagement.Common.Logging.Core;
using Binarysharp.MemoryManagement.Managment;
using Binarysharp.MemoryManagement.Managment.Builders;

namespace Binarysharp.MemoryManagement.Common.Plugins
{
    /// <summary>
    ///     Class PluginManager. This class cannot be inherited.
    /// </summary>
    public sealed class PluginManager : SafeManager<IPlugin>
    {
        /// <summary>
        ///     The thread-safe singleton of the plugin manager. The object is created when any static members/functions of this
        ///     class is called.
        /// </summary>
        public static readonly PluginManager Instance = new PluginManager();

        /// <summary>
        ///     Prevents a default instance of the <see cref="PluginManager" /> class from being created.
        /// </summary>
        private PluginManager() : base(FileLog.Create("PluginManager", "PluginLogs", "PluginLog", true))
        {
        }

        /// <summary>
        ///     Loads the plugins.
        /// </summary>
        public void LoadPlugins()
        {
            if (!Directory.Exists(ApplicationInfo.ApplicationPath + "\\Plugins"))
            {
                Directory.CreateDirectory(ApplicationInfo.ApplicationPath + "\\Plugins");
            }
            // Clear internal items.
            RemoveAll();
            var plugins = Directory.GetFiles(ApplicationInfo.ApplicationPath + "\\Plugins", "*.dll");
            foreach (var plugin in plugins)
            {
                try
                {
                    var asm = Assembly.LoadFrom(plugin);
                    // The filter is set to make sure the instance can be created.
                    foreach (
                        var instance in
                            asm.GetTypes()
                                .Where(bin => bin.IsClass && !bin.IsAbstract && typeof (IPlugin).IsAssignableFrom(bin))
                                .Select(Activator.CreateInstance)
                                .OfType<IPlugin>())
                    {
                        instance.Initialize();
                        InternalItems[instance.Name] = new PluginContainer(instance);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning("Failed to load Plugin: " + plugin + Environment.NewLine +
                                   ex.ExtractDetailedInformation());
                }
            }
        }
    }
}