/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.RemoteProcess.Windows.Mouse
{
    /// <summary>
    ///     Class defining a virtual mouse using the API SendInput.
    /// </summary>
    public class SendInputMouse : BaseMouse
    {
        /// <summary>
        ///     Initializes a new instance of a child of the <see cref="SendInputMouse" /> class.
        /// </summary>
        /// <param name="window">The reference of the <see cref="RemoteWindow" /> object.</param>
        public SendInputMouse(RemoteWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Moves the cursor at the specified coordinate.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        protected override void MoveToAbsolute(int x, int y)
        {
            var input = CreateInput();
            input.Mouse.DeltaX = CalculateAbsoluteCoordinateX(x);
            input.Mouse.DeltaY = CalculateAbsoluteCoordinateY(y);
            input.Mouse.Flags = MouseFlags.Move | MouseFlags.Absolute;
            input.Mouse.MouseData = 0;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Presses the left button of the mouse at the current cursor position.
        /// </summary>
        public override void PressLeft()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.LeftDown;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Presses the middle button of the mouse at the current cursor position.
        /// </summary>
        public override void PressMiddle()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.MiddleDown;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Presses the right button of the mouse at the current cursor position.
        /// </summary>
        public override void PressRight()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.RightDown;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Releases the left button of the mouse at the current cursor position.
        /// </summary>
        public override void ReleaseLeft()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.LeftUp;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Releases the middle button of the mouse at the current cursor position.
        /// </summary>
        public override void ReleaseMiddle()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.MiddleUp;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Releases the right button of the mouse at the current cursor position.
        /// </summary>
        public override void ReleaseRight()
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.RightUp;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Scrolls horizontally using the wheel of the mouse at the current cursor position.
        /// </summary>
        /// <param name="delta">The amount of wheel movement.</param>
        public override void ScrollHorizontally(int delta = 120)
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.HWheel;
            input.Mouse.MouseData = delta;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Scrolls vertically using the wheel of the mouse at the current cursor position.
        /// </summary>
        /// <param name="delta">The amount of wheel movement.</param>
        public override void ScrollVertically(int delta = 120)
        {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.Wheel;
            input.Mouse.MouseData = delta;
            WindowCore.SendInput(input);
        }

        /// <summary>
        ///     Calculates the x-coordinate with the system metric.
        /// </summary>
        private int CalculateAbsoluteCoordinateX(int x)
        {
            return (x*65536)/SafeNativeMethods.GetSystemMetrics(SystemMetrics.CxScreen);
        }

        /// <summary>
        ///     Calculates the y-coordinate with the system metric.
        /// </summary>
        private int CalculateAbsoluteCoordinateY(int y)
        {
            return (y*65536)/SafeNativeMethods.GetSystemMetrics(SystemMetrics.CyScreen);
        }

        /// <summary>
        ///     Create an <see cref="Input" /> structure for mouse event.
        /// </summary>
        private Input CreateInput()
        {
            return new Input(InputTypes.Mouse);
        }
    }
}