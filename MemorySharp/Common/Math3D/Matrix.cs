using System;

namespace Binarysharp.MemoryManagement.Common.Math3D
{
    /// <summary>
    ///     Class Matrix.
    /// </summary>
    public struct Matrix
    {
        private readonly int _columns;

        private readonly float[] _data;
        private readonly int _rows;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix" /> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        public Matrix(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _data = new float[rows*columns];
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Single" /> with the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.Single.</returns>
        public float this[int i]
        {
            get { return _data[i]; }
            set { _data[i] = value; }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Single" /> with the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>System.Single.</returns>
        public float this[int row, int column]
        {
            get { return _data[row*_columns + column]; }
            set { _data[row*_columns + column] = value; }
        }

        /// <summary>
        ///     Reads the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Read(byte[] data)
        {
            for (var y = 0; y < _rows; y++)
                for (var x = 0; x < _columns; x++)
                    this[y, x] = BitConverter.ToSingle(data, sizeof (float)*((y*_columns) + x));
        }

        /// <summary>
        ///     To the byte array.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public byte[] ToByteArray()
        {
            var sof = sizeof (float);
            var data = new byte[_data.Length*sof];
            for (var i = 0; i < _data.Length; i++)
                Array.Copy(BitConverter.GetBytes(_data[i]), 0, data, i*sof, sof);
            return data;
        }
    }
}