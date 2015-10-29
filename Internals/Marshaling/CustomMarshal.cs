using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MemorySharp.Internals.Marshaling
{
    public static class CustomMarshal
    {
        #region Methods
        /// <summary>
        ///     Determines if a given type is CustomMarshalable (i.e. contains any fields with
        ///     <see cref="CustomMarshalAsAttribute" />)
        /// </summary>
        /// <param name="t">Type to check</param>
        /// <returns>True if <paramref name="t" /> contains <see cref="CustomMarshalAsAttribute" /> on any fields</returns>
        public static bool IsCustomMarshalType(Type t)
        {
            foreach (var f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (!f.IsDefined(typeof (CustomMarshalAttribute), true))
                    continue;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether a specific object is CustomMarshalable
        /// </summary>
        /// <param name="o">Object to check</param>
        /// <returns>True if <paramref name="o" /> is CustomMarshalable</returns>
        public static bool IsCustomMarshalObject(object o)
        {
            return IsCustomMarshalType(o.GetType());
        }

        /// <summary>
        ///     Returns the runtime size of a CustomMarshalable object. If object is not CustomMarshalable, this function
        ///     is the same as <see cref="Marshal.SizeOf" />
        /// </summary>
        /// <param name="o">Object to calculate runtime size of</param>
        /// <returns>Size in bytes of <paramref name="o" /></returns>
        public static int SizeOf(object o)
        {
            if (!IsCustomMarshalObject(o))
                return SizeOf(o.GetType());
            var objectSize = Marshal.SizeOf(o);
            foreach (
                var field in o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                )
            {
                if (!field.IsDefined(typeof (CustomMarshalAttribute), true))
                    continue;
                foreach (var attribute in field.GetCustomAttributes(typeof (CustomMarshalAttribute), true))
                {
                    var size = 0;
                    var attr = (CustomMarshalAsAttribute) attribute;
                    switch (attr.Value)
                    {
                        case CustomUnmanagedType.LPStr:
                            var val = (string) field.GetValue(o) + '\0';
                            size = Encoding.ASCII.GetByteCount(val);
                            break;
                        case CustomUnmanagedType.LPWStr:
                            val = (string) field.GetValue(o) + '\0';
                            size = Encoding.Unicode.GetByteCount(val);
                            break;
                        default:
                            throw new NotSupportedException("Operation not yet supported by CustomMarshaller");
                    }
                    objectSize += size;
                }
            }
            return objectSize;
        }

        public static int SizeOf(Type t)
        {
            return Marshal.SizeOf(t);
        }

        /// <summary>
        ///     Marshal a managed object to an unmanaged chunk of memory. If <paramref name="structure" /> is not CustomMarshalable
        ///     this
        ///     function is the same as <see cref="Marshal.StructureToPtr" />.
        /// </summary>
        /// <param name="structure">A managed object to marshal to unmanaged memory.</param>
        /// <param name="ptr">Pointer to unmanaged memory. This must be allocated before call</param>
        /// <param name="fDeleteOld">Indicates whether to delete old memory first</param>
        public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld)
        {
            if (!IsCustomMarshalObject(structure))
            {
                Marshal.StructureToPtr(structure, ptr, fDeleteOld);
                return;
            }

            // first check that the struct has the struct layout attribute
            var sla = structure.GetType().StructLayoutAttribute;
            if (sla.IsDefaultAttribute() || sla.Value == LayoutKind.Auto)
                throw new ArgumentException(
                    "Structure must have StructLayoutAttribute with LayoutKind Explicit or Sequential",
                    nameof(structure));

            // iterate through all struct fields, handling customs, and using Marshal.StructToPtr for others
            uint extraDataOffset = 0;
            var structBase = (uint) ptr.ToInt32();
            var structSize = (uint) Marshal.SizeOf(structure);

            foreach (
                var field in
                    structure.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldLoc = structBase + (uint) Marshal.OffsetOf(structure.GetType(), field.Name);
                if (field.IsDefined(typeof (CustomMarshalAsAttribute), true))
                {
                    byte[] bytes;
                    var attr =
                        (CustomMarshalAsAttribute)
                            (field.GetCustomAttributes(typeof (CustomMarshalAsAttribute), true)[0]);
                    switch (attr.Value)
                    {
                        case CustomUnmanagedType.LPStr:
                            var val = (string) field.GetValue(structure) + '\0';
                            bytes = Encoding.ASCII.GetBytes(val);
                            break;
                        case CustomUnmanagedType.LPWStr:
                            val = (string) field.GetValue(structure) + '\0';
                            bytes = Encoding.Unicode.GetBytes(val);
                            break;
                        default:
                            throw new NotSupportedException("Operation not yet supported");
                    }
                    var dataLoc = structBase + structSize + extraDataOffset;
                    Marshal.WriteIntPtr(new IntPtr(fieldLoc), new IntPtr(dataLoc));
                    // write the raw bytes to dataLoc
                    for (var i = 0; i < bytes.Length; i++, extraDataOffset++)
                    {
                        Marshal.WriteByte(new IntPtr(dataLoc + (uint) i), bytes[i]);
                    }
                }
                else
                {
                    Marshal.StructureToPtr(field.GetValue(structure), new IntPtr(fieldLoc), fDeleteOld);
                }
            }
        }

        /// <summary>
        ///     Marshal an unmanaged to pointer to a managed object. If <paramref name="structureType" /> is not CustomMarshalable,
        ///     this
        ///     function is the same as <see cref="Marshal.StructureToPtr" />.
        /// </summary>
        /// <param name="ptr">Pointer to unmanaged memory</param>
        /// <param name="structureType">Type of managed object to instantiate</param>
        /// <returns>Managed instance of <paramref name="structureType" /> type</returns>
        public static object PtrToStructure(IntPtr ptr, Type structureType)
        {
            if (ptr == IntPtr.Zero)
                return null;
            if (structureType == null)
                throw new ArgumentNullException(nameof(structureType));
            if (structureType.IsGenericType)
                throw new ArgumentException("Structure type must be non-generic", nameof(structureType));

            if (!IsCustomMarshalType(structureType))
                return Marshal.PtrToStructure(ptr, structureType);

            var sla = structureType.StructLayoutAttribute;
            if (sla.IsDefaultAttribute() || sla.Value == LayoutKind.Auto)
                throw new ArgumentException(
                    "Structure must have StructLayoutAttribute with LayoutKind Explicit or Sequential", "structure");

            var structure = Activator.CreateInstance(structureType);

            var structBase = (uint) ptr.ToInt32();
            var structSize = (uint) Marshal.SizeOf(structure);
            foreach (
                var field in
                    structureType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldLoc = structBase + (uint) Marshal.OffsetOf(structureType, field.Name);
                if (field.IsDefined(typeof (CustomMarshalAsAttribute), true))
                {
                    var extraDataLoc = Marshal.ReadIntPtr(new IntPtr(fieldLoc));

                    var attr =
                        (CustomMarshalAsAttribute)
                            (field.GetCustomAttributes(typeof (CustomMarshalAsAttribute), true)[0]);
                    switch (attr.Value)
                    {
                        case CustomUnmanagedType.LPStr:
                            field.SetValue(structure, Marshal.PtrToStringAnsi(extraDataLoc));
                            break;
                        case CustomUnmanagedType.LPWStr:
                            field.SetValue(structure, Marshal.PtrToStringUni(extraDataLoc));
                            break;
                        default:
                            throw new NotSupportedException("Operation not currently supported");
                    }
                }
                else
                {
                    field.SetValue(structure, Marshal.PtrToStructure(new IntPtr(fieldLoc), field.FieldType));
                }
            }
            return structure;
        }

        /// <summary>
        ///     Rebases all pointers in a CustomMarshalable structure to be valid pointers in a target address space. If
        ///     <paramref name="structureType" /> is not CustomMarshalable, this function does nothing.
        /// </summary>
        /// <param name="baseAddress">Pointer to unmanaged structure in this process</param>
        /// <param name="targetAddress">Base address of structure in target process</param>
        /// <param name="structureType">Type of structure</param>
        public static void RebaseUnmanagedStructure(IntPtr baseAddress, IntPtr targetAddress, Type structureType)
        {
            if (baseAddress == IntPtr.Zero)
                throw new ArgumentException("Invalid base address", nameof(baseAddress));
            if (targetAddress == IntPtr.Zero)
                throw new ArgumentException("Invalid target address", nameof(targetAddress));
            if (structureType == null)
                throw new ArgumentNullException(nameof(structureType));
            if (!IsCustomMarshalType(structureType)) // not CustomMarshalType - don't need to / can't rebase
                return;

            var addressDiff = targetAddress.ToInt32() - baseAddress.ToInt32();
            foreach (
                var field in
                    structureType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.IsDefined(typeof (CustomMarshalAsAttribute), true))
                {
                    var fieldAddr =
                        new IntPtr(baseAddress.ToInt32() + Marshal.OffsetOf(structureType, field.Name).ToInt32());
                    var current = Marshal.ReadIntPtr(fieldAddr);
                    var newLoc = new IntPtr(current.ToInt32() + addressDiff);
                    Marshal.WriteIntPtr(fieldAddr, newLoc);
                }
            }
        }
        #endregion
    }
}