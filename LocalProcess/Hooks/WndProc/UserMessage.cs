namespace Binarysharp.MemoryManagement.LocalProcess.Hooks.WndProc
{
    /// <summary>
    ///     Usermessage used for the <see cref="WindowHook" /> class.
    /// </summary>
    public enum UserMessage
    {
        /// <summary>
        ///     Used as the message to invoke <code>PulseEngine.StartUp()</code> method.
        /// </summary>
        StartUp,

        /// <summary>
        ///     Used as the message to invoke <code>PulseEngine.ShutDown()</code> method.
        /// </summary>
        ShutDown
    }
}