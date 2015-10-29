namespace MemorySharp.Internals.Marshaling
{
    public enum CustomUnmanagedType
    {
        /// <summary>
        ///     Pointer to a null-terminated ANSI string
        /// </summary>
        LPStr,

        /// <summary>
        ///     Pointer to a null-terminated Unicode string
        /// </summary>
        LPWStr
    }

    public class CustomMarshalAsAttribute : CustomMarshalAttribute
    {
        #region Constructors
        public CustomMarshalAsAttribute(CustomUnmanagedType unmanagedType)
        {
            Value = unmanagedType;
        }

        public CustomMarshalAsAttribute(short unmanagedType)
        {
            Value = (CustomUnmanagedType) unmanagedType;
        }
        #endregion

        #region  Properties
        public CustomUnmanagedType Value { get; }
        #endregion
    }
}