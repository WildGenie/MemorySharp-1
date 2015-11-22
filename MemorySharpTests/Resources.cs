/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2013 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Binarysharp.MemoryManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace MemorySharpTests
{
    internal static class Resources
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static Resources()
        {
            if(TestProcess32Bit == null)
                throw new Exception("You probably forgot to launch the test process.");
            _path = TestProcess32Bit.MainModule.FileName;
        }

        /// <summary>
        /// The path of the test process.
        /// </summary>
        private static readonly string _path;

        /// <summary>
        /// A new instance of the <see cref="Point"/> structure.
        /// </summary>
        internal static Point CustomStruct => new Point { X = 4, Y = 5, Z = 6 };

        /// <summary>
        /// Provides the path of a test library (dll).
        /// </summary>
        internal static string LibraryTest => Path.Combine(Environment.SystemDirectory, "apphelp.dll");

        /// <summary>
        /// A new instance of the <see cref="MemorySharp"/> class (32-bit).
        /// </summary>
        internal static MemorySharp MemorySharp32Bit => new MemorySharp(TestProcess32Bit);

        /// <summary>
        /// A new instance of the <see cref="MemorySharp"/> class (64-bit).
        /// </summary>
        internal static MemorySharp MemorySharp64Bit => new MemorySharp(TestProcess64Bit);

        /// <summary>
        /// The current process.
        /// </summary>
        internal static Process CurrentProcess => Process.GetCurrentProcess();

        /// <summary>
        /// The process used for tests (32-bit).
        /// </summary>
        internal static Process TestProcess32Bit => Process.GetProcessesByName("notepad++").FirstOrDefault();

        /// <summary>
        /// The process used for tests (64-bit).
        /// </summary>
        internal static Process TestProcess64Bit => Process.GetProcessesByName("calc").FirstOrDefault();

        /// <summary>
        /// Performs the ending tests.
        /// </summary>
        internal static void EndTests(MemorySharp memorySharp)
        {
            Assert.AreEqual(0, memorySharp.Memory.RemoteAllocations.Count());
            Assert.AreEqual(0, memorySharp.Modules.InjectedModules.Count());
        }

        /// <summary>
        /// Restart the test process.
        /// </summary>
        internal static void Restart()
        {
            // Kill the process
            if (TestProcess32Bit != null)
                TestProcess32Bit.Kill();
            // Start it
            Process.Start(_path);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Interrupts the current test if the operating system is not running under an 64-bit operating system.
        /// </summary>
        internal static void InterruptWhenNot64BitOs()
        {
            if (!Environment.Is64BitOperatingSystem)
                Assert.Inconclusive("The operating system is not 64-bit.");
        }
    }

    /// <summary>
    /// A sample test structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IEquatable<Point>
    {
        public int X;
        public int Y;
        public int Z;

        #region Equality methods
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
        #endregion
    }
}
