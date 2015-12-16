/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binarysharp.MemoryManagement.Native.Enums;

namespace Binarysharp.MemoryManagement.Windows.Keyboard
{
    /// <summary>
    ///     Abstract class defining a virtual keyboard.
    /// </summary>
    public abstract class BaseKeyboard
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The collection storing the current pressed keys.
        /// </summary>
        protected static readonly List<Tuple<IntPtr, Keys>> PressedKeys = new List<Tuple<IntPtr, Keys>>();

        /// <summary>
        ///     The reference of the <see cref="RemoteWindow" /> object.
        /// </summary>
        protected readonly RemoteWindow Window;
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of a child of the <see cref="BaseKeyboard" /> class.
        /// </summary>
        /// <param name="window">The reference of the <see cref="RemoteWindow" /> object.</param>
        protected BaseKeyboard(RemoteWindow window)
        {
            // Save the parameter
            Window = window;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Presses the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        public abstract void Press(Keys key);

        /// <summary>
        ///     Writes the specified character to the window.
        /// </summary>
        /// <param name="character">The character to write.</param>
        public abstract void Write(char character);

        /// <summary>
        ///     Releases the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to release.</param>
        public virtual void Release(Keys key)
        {
            // Create the tuple
            var tuple = Tuple.Create(Window.Handle, key);

            // If the key is pressed with an interval
            if (PressedKeys.Contains(tuple))
                PressedKeys.Remove(tuple);
        }

        /// <summary>
        ///     Presses the specified virtual key to the window at a specified interval.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        /// <param name="interval">The interval between the key activations.</param>
        public void Press(Keys key, TimeSpan interval)
        {
            // Create the tuple
            var tuple = Tuple.Create(Window.Handle, key);

            // If the key is already pressed
            if (PressedKeys.Contains(tuple))
                return;

            // Add the key to the collection
            PressedKeys.Add(tuple);
            // Start a new task to press the key at the specified interval
            Task.Run(async () =>
            {
                // While the key must be pressed
                while (PressedKeys.Contains(tuple))
                {
                    // Press the key
                    Press(key);
                    // Wait the interval
                    await Task.Delay(interval);
                }
            });
        }

        /// <summary>
        ///     Presses and releaes the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press and release.</param>
        public void PressRelease(Keys key)
        {
            Press(key);
            Thread.Sleep(10);
            Release(key);
        }

        /// <summary>
        ///     Writes the text representation of the specified array of objects to the window using the specified format
        ///     information.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="args">An array of objects to write using format.</param>
        public void Write(string text, params object[] args)
        {
            foreach (var character in string.Format(text, args))
            {
                Write(character);
            }
        }
        #endregion
    }
}