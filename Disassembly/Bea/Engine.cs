using System.Runtime.InteropServices;

namespace MemorySharp.Disassembly.Bea
{
    public class BeaEngine
    {
        public static string Version => BeaEngineVersion();
        public static string Revision => BeaEngineRevision();

        [DllImport("BeaEngine.dll")]
        public static extern int Disasm([In, Out, MarshalAs(UnmanagedType.LPStruct)] Disasm disasm);

        [DllImport("BeaEngine.dll")]
        private static extern string BeaEngineVersion();

        [DllImport("BeaEngine.dll")]
        private static extern string BeaEngineRevision();
    }
}