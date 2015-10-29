using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MemorySharp.Disassembly.Bea;
using MemorySharp.Memory;

namespace MemorySharp.Disassembly
{
    public class Disassembler
    {
        #region  Fields
        private readonly InternalMemorySharp _memory;
        #endregion

        #region Constructors
        public Disassembler(InternalMemorySharp memorySharp)
        {
            _memory = memorySharp;
        }
        #endregion

        #region  Properties
        private RemoteAllocation Allocation { get; set; }
        #endregion

        #region Methods
        public IEnumerable<DisassemblyInstruction> Disassemble(IntPtr pAddr, int maxInstructionCount = 0)
        {
            if (pAddr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pAddr));

            const int maxInstructionSize = 15;
            const int maxBufferSize = 4096;
            // default buffer of 100 x maxInstructionSize bytes (1500 bytes)
            // unless maxInstructionCount is specified
            var bufferSize = (maxInstructionCount == 0 ? 100 : maxInstructionCount)*maxInstructionSize;
            // ensure we don't allocate too large a buffer if a huge instruction count is specified
            bufferSize = Math.Min(bufferSize, maxBufferSize);

            // allocate an unmanaged buffer (instead of reading into managed byte array)
            var pBuffer = IntPtr.Zero;
            try
            {
                Allocation = _memory.Memory.Allocate(bufferSize);
                pBuffer = Allocation.BaseAddress;

                // TODO: this is probably horribly inefficient
                Marshal.Copy(_memory.ReadArray<byte>(pAddr, bufferSize), 0, pBuffer, bufferSize);

                var pDisasmLoc = pBuffer;
                var virtualAddr = pAddr; // TODO: currently doesnt support x64
                var disasm = new Disasm {EIP = pDisasmLoc, VirtualAddr = virtualAddr};

                int length;
                var instructionsRead = 0;
                var bufferOffset = 0;
                while ((length = BeaEngine.Disasm(disasm)) != (int) BeaConstants.SpecialInfo.UNKNOWN_OPCODE)
                {
                    instructionsRead++;

                    var disasmInstr = new DisassemblyInstruction(disasm, length,
                        _memory.ReadArray<byte>(virtualAddr, length));
                    yield return disasmInstr;

                    pDisasmLoc += length;
                    virtualAddr += length;
                    bufferOffset += length;

                    if (maxInstructionCount > 0 && instructionsRead >= maxInstructionCount)
                        break;

                    // if we don't have an instruction limit and we're less than maxInstructionSize away
                    // from the end of the buffer, reread buffer data from current location
                    if ((bufferSize - bufferOffset) < maxInstructionSize)
                    {
                        // Copy new bytes to buffer from current location
                        Marshal.Copy(_memory.ReadArray<byte>(virtualAddr, bufferSize), 0, pBuffer, bufferSize);
                        // reset pointers etc
                        pDisasmLoc = pBuffer;
                        bufferOffset = 0;
                    }

                    disasm.EIP = pDisasmLoc;
                    disasm.VirtualAddr = virtualAddr;
                }
            }
            finally
            {
                if (pBuffer != IntPtr.Zero)
                    _memory.Memory.Deallocate(Allocation);
            }
        }
        #endregion
    }

    public class DisassemblyInstruction
    {
        #region Constructors
        public DisassemblyInstruction(Disasm disasm, int length, byte[] instructionData)
        {
            Length = length;
            InstructionData = instructionData;
            CompleteInstruction = disasm.CompleteInstr;
            Architecture = disasm.Archi;
            Options = disasm.Options;
            Instruction = disasm.Instruction;
            Argument1 = disasm.Argument1;
            Argument2 = disasm.Argument2;
            Argument3 = disasm.Argument3;
            Prefix = disasm.Prefix;
        }
        #endregion

        #region  Properties

        // Bea Disasm property duplicates
        public string CompleteInstruction { get; private set; }
        public IntPtr Architecture { get; private set; }
        public IntPtr Options { get; private set; }
        public InstructionType Instruction { get; private set; }
        public ArgumentType Argument1 { get; private set; }
        public ArgumentType Argument2 { get; private set; }
        public ArgumentType Argument3 { get; private set; }
        public PrefixInfo Prefix { get; private set; }

        // Extra information
        public int Length { get; private set; }
        public byte[] InstructionData { get; private set; }
        #endregion
    }
}