using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MemorySharp.Extensions;

namespace MemorySharp.Internals
{
    public class Detour<T> : IApplicableElement where T : class
    {
        static Detour()
        {
            if (!typeof (T).IsSubclassOf(typeof (Delegate)) ||
                typeof (T).GetCustomAttributes(typeof (UnmanagedFunctionPointerAttribute), true).Length == 0)
            {
                throw new ArgumentException(
                    "Type T must be a delegate type adorned with the UnmanagedFunctionPointer attribute");
            }
        }

        public Detour(InternalMemorySharp memorySharp, IntPtr targetAddr, T detourFunc)
        {
            JumpSize = IntPtr.Size == 4 ? 5 : 14;
            Memory = memorySharp;
            TargetAddr = targetAddr;
            TargetDelegate = memorySharp.GetDelegate<T>(TargetAddr);
            HookAddr = ((Delegate) (object) detourFunc).ToFunctionPointer();
            // store the original function bytes
            Original = new List<byte>(memorySharp.ReadArray<byte>(TargetAddr, JumpSize));
        }

        private IntPtr HookAddr { get; }
        private int JumpSize { get; } // TODO: x64 support (14 bytes)
        private InternalMemorySharp Memory { get; }
        private List<byte> Original { get; }
        private IntPtr TargetAddr { get; }
        private T TargetDelegate { get; }
        private IntPtr Trampoline { get; set; }
        public bool IsEnabled { get; private set; }

        public T CallOriginal
        {
            get
            {
                if (!IsEnabled) return TargetDelegate;
                if (Trampoline == IntPtr.Zero)
                    throw new InvalidOperationException("No trampoline has been allocated - something has gone wrong");

                return Memory.GetDelegate<T>(Trampoline);
            }
        }

        public bool IsDisposed { get; private set; }
        public bool MustBeDisposed { get; } = true;

        private void WriteJump(IntPtr pAddr, IntPtr pJmpTarget)
        {
            var jmpBytes = new List<byte> {0xE9};

            // relocate address
            var relocAddr = pJmpTarget.ToInt32() - pAddr.ToInt32() - 5;

            jmpBytes.AddRange(BitConverter.GetBytes(relocAddr));

            Memory.WriteArray(pAddr, jmpBytes.ToArray());
        }

        public void Enable()
        {
            if (IsEnabled)
                return;
            // TODO: handle this situation
            if (Trampoline != IntPtr.Zero)
                throw new InvalidOperationException("Trampoline was not correctly freed");
            Trampoline = Memory.Memory.Allocate(JumpSize*3).BaseAddress;
            var pTramp = Trampoline;

            // dissassemble from target address
            // moving JumpSize bytes to trampoline
            var instrByteCount = 0;
            foreach (var instr in Memory.Disassembler.Disassemble(TargetAddr))
            {
                // TODO: work out jumps
                Memory.WriteArray(pTramp, instr.InstructionData);
                pTramp += instr.Length;
                instrByteCount += instr.Length;

                // We now have enough data, stop disassembling
                if (instrByteCount >= JumpSize)
                    break;
            }

            // write a jmp instruction from trampoline back to function
            WriteJump(pTramp, TargetAddr + instrByteCount);

            // Write jump from target to detour
            WriteJump(TargetAddr, HookAddr);

            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled)
                return;

            // Restore the original bytes to the function address
            Memory.WriteArray(TargetAddr, Original.ToArray());

            // Release the trampoline
            Memory.FreeMemory(Memory.Handle, Trampoline);
        }

        public void Dispose()
        {
            if (!MustBeDisposed)
                return;
            Disable();

            // Ensure _trampoline is freed, even though this should happen in Remove()
            if (Trampoline != IntPtr.Zero)
            {
                Memory.FreeMemory(Memory.Handle, Trampoline);
            }
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~Detour()
        {
            Dispose();
        }
    }
}