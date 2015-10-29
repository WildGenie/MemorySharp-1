using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MemorySharp.Internals;
using MemorySharp.Internals.Marshaling;
using MemorySharp.Modules;
using MemorySharp.Native;

namespace MemorySharp.Extensions
{
    /// <summary>
    ///     A class providing extension methods for <see cref="Process" /> Instance's.
    ///     <remarks>
    ///         eturns>Unfinshed documentation. Most credits go to: "Jeffora"'s extememory project.
    ///         https://github.com/jeffora/extemory
    ///     </remarks>
    /// </summary>
    public static class ProcessExtensions
    {
        #region  Fields
        private const int MaxStringSizeBytes = 1024;
        #endregion

        #region Methods
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static unsafe T Read<T>(this Process proc, IntPtr addr) where T : struct
        {
            var type = MarshalType<T>.RealType;
            var size = MarshalType<T>.Size;
            var bytes = new byte[size];
            int bytesRead;
            if (!NativeMethods.ReadProcessMemory(proc.GetHandle(), addr, bytes, (uint) size, out bytesRead))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (bytesRead != size)
                throw new Exception($"Unable to read {size} bytes from process for intput type {type.Name}");

            object ret;
            if (MarshalType<T>.IsIntPtr)
            {
                fixed (byte* pByte = bytes)
                    ret = new IntPtr(*(void**) pByte);
                return (T) ret;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    ret = BitConverter.ToBoolean(bytes, 0);
                    break;
                case TypeCode.Char:
                    ret = BitConverter.ToChar(bytes, 0);
                    break;
                case TypeCode.Byte:
                    ret = bytes[0];
                    break;
                case TypeCode.SByte:
                    ret = (sbyte) bytes[0];
                    break;
                case TypeCode.Int16:
                    ret = BitConverter.ToInt16(bytes, 0);
                    break;
                case TypeCode.Int32:
                    ret = BitConverter.ToInt32(bytes, 0);
                    break;
                case TypeCode.Int64:
                    ret = BitConverter.ToInt64(bytes, 0);
                    break;
                case TypeCode.UInt16:
                    ret = BitConverter.ToUInt16(bytes, 0);
                    break;
                case TypeCode.UInt32:
                    ret = BitConverter.ToUInt32(bytes, 0);
                    break;
                case TypeCode.UInt64:
                    ret = BitConverter.ToUInt64(bytes, 0);
                    break;
                case TypeCode.Single:
                    ret = BitConverter.ToSingle(bytes, 0);
                    break;
                case TypeCode.Double:
                    ret = BitConverter.ToDouble(bytes, 0);
                    break;
                default:
                    throw new NotSupportedException(type.FullName + " is not supported yet");
                // TODO: general struct
            }

            return (T) ret;
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static void Write<T>(this Process proc, IntPtr addr, T value) where T : struct
        {
            object val = value;

            if (MarshalType<T>.IsIntPtr)
            {
                val = IntPtr.Size == 8 ? ((IntPtr) val).ToInt64() : ((IntPtr) val).ToInt32();
            }

            byte[] bytes;
            switch (Type.GetTypeCode(val.GetType()))
            {
                case TypeCode.Boolean:
                    bytes = BitConverter.GetBytes((bool) val);
                    break;
                case TypeCode.Byte:
                    bytes = new[] {(byte) val};
                    break;
                case TypeCode.SByte:
                    bytes = new[] {(byte) ((sbyte) val)};
                    break;
                case TypeCode.Char:
                    bytes = BitConverter.GetBytes((char) val);
                    break;
                case TypeCode.Int16:
                    bytes = BitConverter.GetBytes((short) val);
                    break;
                case TypeCode.UInt16:
                    bytes = BitConverter.GetBytes((ushort) val);
                    break;
                case TypeCode.Int32:
                    bytes = BitConverter.GetBytes((int) val);
                    break;
                case TypeCode.UInt32:
                    bytes = BitConverter.GetBytes((uint) val);
                    break;
                case TypeCode.Int64:
                    bytes = BitConverter.GetBytes((long) val);
                    break;
                case TypeCode.UInt64:
                    bytes = BitConverter.GetBytes((ulong) val);
                    break;
                case TypeCode.Single:
                    bytes = BitConverter.GetBytes((float) val);
                    break;
                case TypeCode.Double:
                    bytes = BitConverter.GetBytes((double) val);
                    break;
                default:
                    throw new ArgumentException($"Unsupported type argument supplied: {val.GetType()}");
            }
            int bytesWritten;
            if (
                !NativeMethods.WriteProcessMemory(proc.GetHandle(), addr, bytes, (uint) bytes.Length, out bytesWritten) ||
                bytesWritten != bytes.Length)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static T[] ReadArray<T>(this Process proc, IntPtr addr, int count) where T : struct
        {
            var dataSize = Marshal.SizeOf(typeof (T))*count;
            var pBytes = Marshal.AllocHGlobal(dataSize);
            try
            {
                int bytesRead;
                if (!NativeMethods.ReadProcessMemory(proc.GetHandle(), addr, pBytes, (uint) dataSize, out bytesRead))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (bytesRead != dataSize)
                    throw new Exception(
                        $"Unable to read {dataSize} bytes for array of type {typeof (T).Name} with {count} elements");

                switch (Type.GetTypeCode(typeof (T)))
                {
                    case TypeCode.Byte:
                        var bytes = new byte[count];
                        Marshal.Copy(pBytes, bytes, 0, count);
                        return bytes.Cast<T>().ToArray();
                    case TypeCode.Char:
                        var chars = new char[count];
                        Marshal.Copy(pBytes, chars, 0, count);
                        return chars.Cast<T>().ToArray();
                    case TypeCode.Int16:
                        var shorts = new short[count];
                        Marshal.Copy(pBytes, shorts, 0, count);
                        return shorts.Cast<T>().ToArray();
                    case TypeCode.Int32:
                        var ints = new int[count];
                        Marshal.Copy(pBytes, ints, 0, count);
                        return ints.Cast<T>().ToArray();
                    case TypeCode.Int64:
                        var longs = new long[count];
                        Marshal.Copy(pBytes, longs, 0, count);
                        return longs.Cast<T>().ToArray();
                    case TypeCode.Single:
                        var floats = new float[count];
                        Marshal.Copy(pBytes, floats, 0, count);
                        return floats.Cast<T>().ToArray();
                    case TypeCode.Double:
                        var doubles = new double[count];
                        Marshal.Copy(pBytes, doubles, 0, count);
                        return doubles.Cast<T>().ToArray();
                    default:
                        throw new ArgumentException($"Unsupported type argument supplied: {typeof (T).Name}");
                }
            }
            finally
            {
                if (pBytes != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pBytes);
                }
            }
        }

        public static void WriteArray<T>(this Process proc, IntPtr addr, T[] data) where T : struct
        {
            var size = data.Length;
            var dataSize = size*Marshal.SizeOf(typeof (T));

            var pTemp = Marshal.AllocHGlobal(dataSize);

            try
            {
                switch (Type.GetTypeCode(typeof (T)))
                {
                    case TypeCode.Byte:
                        var bytes = data.Cast<byte>().ToArray();
                        Marshal.Copy(bytes, 0, pTemp, size);
                        break;
                    case TypeCode.Char:
                        var chars = data.Cast<char>().ToArray();
                        Marshal.Copy(chars, 0, pTemp, size);
                        break;
                    case TypeCode.Int16:
                        var shorts = data.Cast<short>().ToArray();
                        Marshal.Copy(shorts, 0, pTemp, size);
                        break;
                    case TypeCode.Int32:
                        var ints = data.Cast<int>().ToArray();
                        Marshal.Copy(ints, 0, pTemp, size);
                        break;
                    case TypeCode.Int64:
                        var longs = data.Cast<long>().ToArray();
                        Marshal.Copy(longs, 0, pTemp, size);
                        break;
                    case TypeCode.Single:
                        var floats = data.Cast<float>().ToArray();
                        Marshal.Copy(floats, 0, pTemp, size);
                        break;
                    case TypeCode.Double:
                        var doubles = data.Cast<double>().ToArray();
                        Marshal.Copy(doubles, 0, pTemp, size);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported type argument supplied: {typeof (T).Name}");
                }

                int bytesWritten;
                if (
                    !NativeMethods.WriteProcessMemory(proc.GetHandle(), addr, pTemp, (uint) dataSize, out bytesWritten) ||
                    bytesWritten != dataSize)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (pTemp != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTemp);
                }
            }
        }

        public static string ReadString(this Process proc, IntPtr addr, Encoding encoding,
            int maxSize = MaxStringSizeBytes)
        {
            if (
                !(encoding.Equals(Encoding.UTF8) || encoding.Equals(Encoding.Unicode) || encoding.Equals(Encoding.ASCII)))
            {
                throw new ArgumentException($"Encoding type {encoding.EncodingName} is not supported", nameof(encoding));
            }

            var bytes = proc.ReadArray<byte>(addr, maxSize);
            var terminalCharacterByte = encoding.GetBytes(new[] {'\0'});
            var buffer = new List<byte>();
            var variableByteSize = encoding.Equals(Encoding.UTF8);
            if (variableByteSize)
            {
                for (var i = 0; i < bytes.Length;)
                {
                    var match = true;
                    var shortBuffer = new List<byte>();
                    for (var j = 0; j < terminalCharacterByte.Length; j++)
                    {
                        shortBuffer.Add(bytes[i + j]);
                        if (bytes[i + j] == terminalCharacterByte[j]) continue;
                        match = false;
                        break;
                    }
                    if (match)
                    {
                        break;
                    }
                    buffer.AddRange(shortBuffer);
                    i += shortBuffer.Count;
                }
            }
            else
            {
                for (var i = 0; i < bytes.Length; i += terminalCharacterByte.Length)
                {
                    var range = new byte[terminalCharacterByte.Length];
                    var match = true;
                    for (var j = 0; j < terminalCharacterByte.Length; j++)
                    {
                        range[j] = bytes[i + j];
                        if (range[j] != terminalCharacterByte[j]) match = false;
                    }
                    if (!match)
                    {
                        buffer.AddRange(range);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var result = encoding.GetString(buffer.ToArray());
            return result;
        }

        public static void WriteString(this Process proc, IntPtr addr, string value, Encoding encoding,
            bool appendNullCharacter = true)
        {
            var bytes = encoding.GetBytes(value);
            if (appendNullCharacter)
            {
                bytes = bytes.Concat(encoding.GetBytes(new[] {'\0'})).ToArray();
            }

            proc.WriteArray(addr, bytes);
        }

        public static IntPtr Allocate(this Process proc, IntPtr addr, uint size,
            MemoryAllocationFlags allocationType = MemoryAllocationFlags.Commit | MemoryAllocationFlags.Reserve,
            MemoryProtectionFlags memoryProtection = MemoryProtectionFlags.ReadWrite)
        {
            if (proc.Id == Process.GetCurrentProcess().Id)
            {
                return NativeMethods.VirtualAlloc(addr, size, allocationType, memoryProtection);
            }
            return NativeMethods.VirtualAllocEx(proc.GetHandle(), addr, size, allocationType, memoryProtection);
        }

        public static bool Free(this Process proc, IntPtr addr, int size = 0,
            MemoryReleaseFlags freeType = MemoryReleaseFlags.Release)
        {
            if (proc.Id == Process.GetCurrentProcess().Id)
            {
                return NativeMethods.VirtualFree(addr, size, freeType);
            }
            return NativeMethods.VirtualFreeEx(proc.GetHandle(), addr, size, freeType);
        }

        public static IEnumerable<AlternateInjectedModule> EnumInjectedModules(this Process proc)
        {
            return ProcessMemoryManager.ForProcess(proc).InjectedModules.Values;
        }

        public static AlternateInjectedModule GetInjectedModule(this Process proc, string moduleName)
        {
            var modules = ProcessMemoryManager.ForProcess(proc).InjectedModules;
            AlternateInjectedModule module;
            if (modules.TryGetValue(moduleName, out module))
                return module;
            //throw new ArgumentException("Invalid module name", "moduleName");
            return null;
        }

        public static AlternateInjectedModule InjectLibrary(this Process proc, string modulePath,
            string moduleName = null)
        {
            if (!File.Exists(modulePath))
            {
                throw new FileNotFoundException("Unable to find the specified module", modulePath);
            }
            modulePath = Path.GetFullPath(modulePath);
            if (string.IsNullOrEmpty(moduleName))
            {
                moduleName = Path.GetFileName(modulePath) ?? modulePath;
            }

            var manager = ProcessMemoryManager.ForProcess(proc);
            if (manager.InjectedModules.ContainsKey(moduleName))
                throw new ArgumentException("Module with this name has already been injected", nameof(moduleName));

            // unmanaged resources that need to be freed
            var pLibRemote = IntPtr.Zero;
            var hThread = IntPtr.Zero;
            var pLibFullPath = Marshal.StringToHGlobalUni(modulePath);

            try
            {
                var sizeUni = Encoding.Unicode.GetByteCount(modulePath);

                // Handle to Kernel32.dll and LoadLibraryW
                var hKernel32 = NativeMethods.GetModuleHandle("Kernel32");
                if (hKernel32 == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var hLoadLib = NativeMethods.GetProcAddress(hKernel32, "LoadLibraryW");
                if (hLoadLib == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // allocate memory to the local process for libFullPath
                pLibRemote = NativeMethods.VirtualAllocEx(proc.GetHandle(), IntPtr.Zero, (uint) sizeUni,
                    MemoryAllocationFlags.Commit,
                    MemoryProtectionFlags.ReadWrite);
                if (pLibRemote == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                int bytesWritten;
                if (
                    !NativeMethods.WriteProcessMemory(proc.GetHandle(), pLibRemote, pLibFullPath, (uint) sizeUni,
                        out bytesWritten) || bytesWritten != sizeUni)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // load dll via call to LoadLibrary using CreateRemoteThread
                hThread = NativeMethods.CreateRemoteThread(proc.GetHandle(), IntPtr.Zero, 0, hLoadLib, pLibRemote, 0,
                    IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (NativeMethods.WaitForSingleObject(hThread, (uint) ThreadWaitValue.Infinite) !=
                    (uint) ThreadWaitValue.Object0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // get address of loaded module - this doesn't work in x64, so just iterate module list to find injected module
                // TODO: fix for x64
                IntPtr hLibModule;
                if (!NativeMethods.GetExitCodeThread(hThread, out hLibModule))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (hLibModule == IntPtr.Zero)
                    throw new Exception("Code executed properly but unable to get get module base address");

                var module = new AlternateInjectedModule(hLibModule, moduleName, proc);
                manager.InjectedModules[moduleName] = module;
                return new AlternateInjectedModule(hLibModule, moduleName, proc);
            }
            finally
            {
                Marshal.FreeHGlobal(pLibFullPath);
                if (hThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(hThread);
                if (pLibRemote != IntPtr.Zero)
                    NativeMethods.VirtualFreeEx(proc.GetHandle(), pLibRemote, 0, MemoryReleaseFlags.Release);
            }
        }

        public static IntPtr GetHandle(this Process proc)
        {
            return ProcessMemoryManager.ForProcess(proc).Handle;
        }

        public static void Resume(this Process proc)
        {
            var hThread = IntPtr.Zero;
            try
            {
                hThread = NativeMethods.OpenThread(ThreadAccessFlags.AllAccess, false,
                    ProcessMemoryManager.ForProcess(proc).MainThreadId).DangerousGetHandle();
                if (hThread == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var hr = NativeMethods.ResumeThread(hThread);
                Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (hThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(hThread);
            }
        }
        #endregion
    }
}