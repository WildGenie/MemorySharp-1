using System;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.Common.Math3D
{
    /// <summary>
    ///     A structure containing values and operations related to a <see cref="Matrix" /> structure with four rows and four
    ///     columns.
    ///     //TODO: Document this further.
    /// </summary>
    public struct Matrix4X4
    {
        /// <summary>
        ///     Gets or sets the M11.
        /// </summary>
        /// <value>
        ///     The M11.
        /// </value>
        public float M11 { get; set; }

        /// <summary>
        ///     Gets or sets the M12.
        /// </summary>
        /// <value>
        ///     The M12.
        /// </value>
        public float M12 { get; set; }

        /// <summary>
        ///     Gets or sets the M13.
        /// </summary>
        /// <value>
        ///     The M13.
        /// </value>
        public float M13 { get; set; }

        /// <summary>
        ///     Gets or sets the M14.
        /// </summary>
        /// <value>
        ///     The M14.
        /// </value>
        public float M14 { get; set; }

        /// <summary>
        ///     Gets or sets the M21.
        /// </summary>
        /// <value>
        ///     The M21.
        /// </value>
        public float M21 { get; set; }

        /// <summary>
        ///     Gets or sets the M22.
        /// </summary>
        /// <value>
        ///     The M22.
        /// </value>
        public float M22 { get; set; }

        /// <summary>
        ///     Gets or sets the M23.
        /// </summary>
        /// <value>
        ///     The M23.
        /// </value>
        public float M23 { get; set; }

        /// <summary>
        ///     Gets or sets the M24.
        /// </summary>
        /// <value>
        ///     The M24.
        /// </value>
        public float M24 { get; set; }

        /// <summary>
        ///     Gets or sets the M31.
        /// </summary>
        /// <value>
        ///     The M31.
        /// </value>
        public float M31 { get; set; }

        /// <summary>
        ///     Gets or sets the M32.
        /// </summary>
        /// <value>
        ///     The M32.
        /// </value>
        public float M32 { get; set; }

        /// <summary>
        ///     Gets or sets the M33.
        /// </summary>
        /// <value>
        ///     The M33.
        /// </value>
        public float M33 { get; set; }

        /// <summary>
        ///     Gets or sets the M34.
        /// </summary>
        /// <value>
        ///     The M34.
        /// </value>
        public float M34 { get; set; }

        /// <summary>
        ///     Gets or sets the M41.
        /// </summary>
        /// <value>
        ///     The M41.
        /// </value>
        public float M41 { get; set; }

        /// <summary>
        ///     Gets or sets the M42.
        /// </summary>
        /// <value>
        ///     The M42.
        /// </value>
        public float M42 { get; set; }

        /// <summary>
        ///     Gets or sets the M43.
        /// </summary>
        /// <value>
        ///     The M43.
        /// </value>
        public float M43 { get; set; }

        /// <summary>
        ///     Gets or sets the M44.
        /// </summary>
        /// <value>
        ///     The M44.
        /// </value>
        public float M44 { get; set; }

        /// <summary>
        ///     Reads the four by four matrix.
        /// </summary>
        /// <param name="handle">The handle to the process where the matrix is located in memory.</param>
        /// <param name="lpBaseAddress">The <see cref="IntPtr" /> address of where the four by four matrix is located in memory.</param>
        /// <returns></returns>
        public static Matrix4X4 ReadFourByFourMatrix(SafeMemoryHandle handle, IntPtr lpBaseAddress)
        {
            var tmp = new Matrix4X4();

            var buffer = MemoryCore.ReadBytes(handle, lpBaseAddress, 64);

            tmp.M11 = BitConverter.ToSingle(buffer, 0*4);
            tmp.M12 = BitConverter.ToSingle(buffer, 1*4);
            tmp.M13 = BitConverter.ToSingle(buffer, 2*4);
            tmp.M14 = BitConverter.ToSingle(buffer, 3*4);

            tmp.M21 = BitConverter.ToSingle(buffer, 4*4);
            tmp.M22 = BitConverter.ToSingle(buffer, 5*4);
            tmp.M23 = BitConverter.ToSingle(buffer, 6*4);
            tmp.M24 = BitConverter.ToSingle(buffer, 7*4);

            tmp.M31 = BitConverter.ToSingle(buffer, 8*4);
            tmp.M32 = BitConverter.ToSingle(buffer, 9*4);
            tmp.M33 = BitConverter.ToSingle(buffer, 10*4);
            tmp.M34 = BitConverter.ToSingle(buffer, 11*4);

            tmp.M41 = BitConverter.ToSingle(buffer, 12*4);
            tmp.M42 = BitConverter.ToSingle(buffer, 13*4);
            tmp.M43 = BitConverter.ToSingle(buffer, 14*4);
            tmp.M44 = BitConverter.ToSingle(buffer, 15*4);
            return tmp;
        }
    }
}