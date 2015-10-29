using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Disassembly.Bea
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class REX_Struct
    {
        #region  Fields
        public byte B_;
        public byte R_;
        public byte state;
        public byte W_;
        public byte X_;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PrefixInfo
    {
        #region  Fields
        public byte AddressSize;
        public byte BranchNotTaken;
        public byte BranchTaken;
        public byte CSPrefix;
        public byte DSPrefix;
        public byte ESPrefix;
        public byte FSPrefix;
        public byte GSPrefix;
        public byte LockPrefix;
        public int NbUndefined;
        public int Number;
        public byte OperandSize;
        public byte RepnePrefix;
        public byte RepPrefix;
        public REX_Struct REX;
        public byte SSPrefix;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class EFLStruct
    {
        #region  Fields
        public byte AF_;
        public byte alignment;
        public byte CF_;
        public byte DF_;
        public byte IF_;
        public byte NT_;
        public byte OF_;
        public byte PF_;
        public byte RF_;
        public byte SF_;
        public byte TF_;
        public byte ZF_;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MemoryType
    {
        #region  Fields
        public int BaseRegister;
        public long Displacement;
        public int IndexRegister;
        public int Scale;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class InstructionType
    {
        #region  Fields
        public ulong AddrValue;
        public int BranchType;
        public int Category;
        public EFLStruct Flags;
        public long Immediat;
        public uint ImplicitModifiedRegs;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)] public string Mnemonic;
        public int Opcode;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ArgumentType
    {
        #region  Fields
        public uint AccessMode;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string ArgMnemonic;
        public int ArgPosition;
        public int ArgSize;
        public int ArgType;
        public MemoryType Memory;
        public uint SegmentReg;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Disasm
    {
        #region  Fields
        public IntPtr Archi;
        public ArgumentType Argument1;
        public ArgumentType Argument2;
        public ArgumentType Argument3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string CompleteInstr;
        public IntPtr EIP;
        public InstructionType Instruction;
        public IntPtr Options;
        public PrefixInfo Prefix;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = UnmanagedType.U4)] private IntPtr[]
            Reserved_;

        public IntPtr SecurityBlock;
        public IntPtr VirtualAddr;
        #endregion
    }
}