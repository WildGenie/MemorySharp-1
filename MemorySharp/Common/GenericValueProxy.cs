using System;

namespace Binarysharp.MemoryManagement.Common
{
    /// <summary>
    ///     Interface that defines methods and values to help get generic values.
    ///     <remarks>
    ///         This is currently an internal interface with low-documentation on purpose. It is not intended for most
    ///         users to use this interface manually.
    ///     </remarks>
    /// </summary>
    internal interface IGenericValue
    {
        #region Public Properties, Indexers
        int FuncHashCode { get; }
        object Result { get; }
        #endregion

        #region Public Methods
        void SetValue();
        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericValueProxy<T> : IGenericValue
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericValueProxy{T}" /> class.
        /// </summary>
        /// <param name="func">The <see cref="Func{TypeResult}" /> that gets the invoked value result desired.</param>
        /// <param name="val">The default value.</param>
        public GenericValueProxy(Func<T> func, T val = default(T))
        {
            Func = func;
            Value = val;
            FuncHashCode = func.GetHashCode();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The initial value, which is later set by the <see cref="SetValue" /> method.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        ///     The <see cref="Value" /> result as an <see cref="objbect" /> instance.
        /// </summary>
        public object Result => Value;

        /// <summary>
        ///     Gets the <see cref="Func{TResult}" /> used to get the invoked value for this instance.
        /// </summary>
        /// <value>
        ///     The function.
        /// </value>
        public Func<T> Func { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is value set.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is value set; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueSet { get; private set; }

        /// <summary>
        ///     The hash code of the <see cref="Func{T}" /> property contained instance.
        /// </summary>
        public int FuncHashCode { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Sets the <see cref="Value" /> property.
        /// </summary>
        public void SetValue()
        {
            if (IsValueSet)
            {
                return;
            }
            Value = Func();
            IsValueSet = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Gets a new <see cref="GenericValueProxy{T}" /> instance.
        /// </summary>
        /// <typeparam name="TT">The value type being invoked.</typeparam>
        /// <param name="func">The function that gets the value.</param>
        /// <param name="value">The default, aka initial value.</param>
        /// <returns>A new <see cref="GenericValueProxy{T}" /> instance.</returns>
        public static GenericValueProxy<TT> Create<TT>(Func<TT> func, TT value = default(TT))
        {
            return new GenericValueProxy<TT>(func, value);
        }
        #endregion
    }
}