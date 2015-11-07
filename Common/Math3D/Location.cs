using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Binarysharp.MemoryManagement.Math3D
{
    /// <summary>
    ///     Struct Location
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Location
    {
        /// <summary>
        ///     The x axis.
        /// </summary>
        public float X;

        /// <summary>
        ///     The y axis.
        /// </summary>
        public float Y;

        /// <summary>
        ///     The z axis.
        /// </summary>
        public float Z;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Location" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Location(float x, float y, float z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Location" /> struct.
        /// </summary>
        /// <param name="xml">The XML.</param>
        public Location(XElement xml)
            : this(Convert.ToSingle(xml.Attribute("X").Value),
                Convert.ToSingle(xml.Attribute("Y").Value),
                Convert.ToSingle(xml.Attribute("Z").Value))
        {
        }

        /// <summary>
        ///     The length.
        /// </summary>
        public double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

        /// <summary>
        ///     Gets the <code>float</code> angle of the vector.
        /// </summary>
        public float Angle => (float) Math.Atan2(Y, X);

        /// <summary>
        ///     Gets a new location instance with 0,0,0 as te values.
        /// </summary>
        public static Location Zero => new Location(0, 0, 0);

        /// <summary>
        ///     Distances to.
        /// </summary>
        /// <param name="loc">The loc.</param>
        /// <returns>System.Double.</returns>
        public double DistanceTo(Location loc)
        {
            return
                Math.Sqrt(Math.Pow(X - loc.X, 2) + Math.Pow(Y - loc.Y, 2) +
                          Math.Pow(Z - loc.Z, 2));
        }

        /// <summary>
        ///     Distance2s the d.
        /// </summary>
        /// <param name="loc">The loc.</param>
        /// <returns>System.Double.</returns>
        public double Distance2D(Location loc)
        {
            return Math.Sqrt(Math.Pow(X - loc.X, 2) + Math.Pow(Y - loc.Y, 2));
        }

        /// <summary>
        ///     Normalizes this instance.
        /// </summary>
        /// <returns>Binarysharp.MemoryManagement.Math.Location.</returns>
        public Location Normalize()
        {
            var len = Length;
            return new Location((float) (X/len), (float) (Y/len), (float) (Z/len));
        }

        /*public T ToVector<T>()
        {
            dynamic vec;

            try
            {
                vec = Activator.CreateInstance(typeof(T), X, Y, Z);
            }
            catch
            {
                vec = default(T);
                if (vec != null)
                {
                    vec.X = X;
                    vec.Y = Y;
                    vec.Z = Z;
                }
            }

            return vec;
        }*/

        /// <summary>
        ///     To the vector3.
        /// </summary>
        /// <returns>Binarysharp.MemoryManagement.Math.Vector3.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        /// <summary>
        ///     Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Boolean.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var loc = (Location) obj;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return loc.X == X && loc.Y == Y && loc.Z == Z;
        }

        /// <summary>
        ///     Gets the hash code.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return X.GetHashCode() | Y.GetHashCode() | Z.GetHashCode();
        }

        /// <summary>
        ///     To the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToString()
        {
            return "[" + (int) X + ", " + (int) Y + ", " + (int) Z + "]";
        }

        /// <summary>
        ///     Gets the XML.
        /// </summary>
        /// <returns>System.Xml.Linq.XElement.</returns>
        public XElement GetXml()
        {
            return new XElement("Location", new XAttribute("X", X), new XAttribute("Y", Y), new XAttribute("Z", Z));
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Location a, Location b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Location a, Location b)
        {
            return !a.Equals(b);
        }
    }
}