using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MemorySharp.Helpers.Extensions;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Native;

namespace MemorySharp.Disassembly
{
    public class BrokenDetours<T> : IApplicableElement where T : class
    {
        #region  Fields
        private readonly IntPtr _hookAddr;
        private readonly List<byte> _original;
        private readonly IntPtr _targetAddr;
        private readonly T _targetDelegate;
        private IntPtr _trampoline;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static uint _jumpSize; // TODO: x64 support (14 bytes)
        #endregion

        #region Constructors
        static BrokenDetours()
        {
            _jumpSize = (uint) (IntPtr.Size == 8 ? 14 : 5);
            if (!typeof (T).IsSubclassOf(typeof (Delegate)) ||
                typeof (T).GetCustomAttributes(typeof (UnmanagedFunctionPointerAttribute), true).Length == 0)
            {
                throw new ArgumentException(
                    "Type T must be a delegate type adorned with the UnmanagedFunctionPointer attribute");
            }
        }

        internal BrokenDetours(IntPtr targetAddr, T detourFunc)
        {
            _targetAddr = targetAddr;
            _targetDelegate = _targetAddr.ToDelegate<T>();
            _hookAddr = ((Delegate) (object) detourFunc).ToFunctionPointer();

            // store the original function bytes
            _original = new List<byte>(_targetAddr.ReadArray<byte>((int) _jumpSize, false));
        }
        #endregion

        #region  Properties
        public bool IsEnabled { get; private set; }

        public T CallOriginal
        {
            get
            {
                if (IsEnabled)
                {
                    if (_trampoline == IntPtr.Zero)
                        throw new InvalidOperationException(
                            "No trampoline has been allocated - something has gone wrong");

                    return _trampoline.ToDelegate<T>();
                }
                return _targetDelegate;
            }
        }

        public bool IsDisposed { get; private set; }
        public bool MustBeDisposed { get; } = true;
        #endregion

        #region  Interface members
        public void Enable()
        {
            if (IsEnabled)
                return;

            // TODO: handle this situation
            if (_trampoline != IntPtr.Zero)
                throw new InvalidOperationException("Trampoline was not correctly freed");

            _trampoline = Process.GetCurrentProcess()
                .Allocate(IntPtr.Zero, _jumpSize*3, memoryProtection: MemoryProtectionFlags.ExecuteReadWrite);
            var pTramp = _trampoline;

            // dissassemble from target address
            // moving JumpSize bytes to trampoline
            var instrByteCount = 0;
            foreach (var instr in _targetAddr.Disassemble())
            {
                // TODO: work out jumps
                pTramp.WriteArray(instr.InstructionData, false);
                pTramp += instr.Length;
                instrByteCount += instr.Length;

                // We now have enough data, stop disassembling
                if (instrByteCount >= _jumpSize)
                    break;
            }

            // write a jmp instruction from trampoline back to function
            WriteJump(pTramp, _targetAddr + instrByteCount);

            // Write jump from target to detour
            WriteJump(_targetAddr, _hookAddr);

            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled)
                return;

            // Restore the original bytes to the function address
            _targetAddr.WriteArray(_original.ToArray(), false);

            // Release the trampoline
            Process.GetCurrentProcess().Free(_trampoline);
        }

        public void Dispose()
        {
            if (!MustBeDisposed)
                return;

            Disable();

            // Ensure _trampoline is freed, even though this should happen in Disable()
            if (_trampoline != IntPtr.Zero)
            {
                Process.GetCurrentProcess().Free(_trampoline);
            }
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        private static void WriteJump(IntPtr pAddr, IntPtr pJmpTarget)
        {
            var jmpBytes = new List<byte> {0xE9};

            // relocate address
            var relocAddr = pJmpTarget.ToInt32() - pAddr.ToInt32() - 5;

            jmpBytes.AddRange(BitConverter.GetBytes(relocAddr));

            pAddr.WriteArray(jmpBytes.ToArray(), false);
        }
        #endregion

        #region Misc
        ~BrokenDetours()
        {
            Dispose();
        }
        #endregion
    }
}