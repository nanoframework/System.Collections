// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Collections.Generic
{

    /// <summary>
    /// Creates instances of the <see cref="KeyValuePair{TKey,TValue}"/> struct.
    /// </summary>
    public static class KeyValuePair
    {
        /// <summary>
        /// Creates a new key/value pair instance using provided values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key"> The key of the new  <see cref="KeyValuePair{TKey,TValue}"/> to be created.</param>
        /// <param name="value"> The value of the new  <see cref="KeyValuePair{TKey,TValue}"/> to be created.</param>
        /// <returns>A key/value pair containing the provided arguments as values.</returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) =>
            new KeyValuePair<TKey, TValue>(key, value);

        ///// <summary>Used by KeyValuePair.ToString to reduce generic code</summary>
        //internal static string PairToString(object? key, object? value) =>
        //    string.Create(null, stackalloc char[256], $"[{key}, {value}]");
    }

    /// <summary>
    /// Defines a key/value pair that can be set or retrieved.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct KeyValuePair<TKey, TValue>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TKey key; // Do not rename (binary serialization)
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TValue value; // Do not rename (binary serialization)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePair{TKey,TValue}"/> structure with the specified key and value.
        /// </summary>
        /// <param name="key">The object defined in each key/value pair.</param>
        /// <param name="value">The definition associated with <paramref name="key"/>.</param>
        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the key in the key/value pair.
        /// </summary>
        /// <value>A TKey that is the key of the <see cref="KeyValuePair{TKey,TValue}"/>.</value> 
        public TKey Key => key;

        /// <summary>
        /// Gets the value in the key/value pair.
        /// </summary>
        public TValue Value => value;

        ///// <summary>
        ///// Returns a string representation of the <see cref="KeyValuePair{TKey,TValue}"/> using the string representations of the key and value.
        ///// </summary>
        ///// <returns>A string representation of the <see cref="KeyValuePair{TKey,TValue}"/>, which includes the string representations of the key and value.</returns>
        //public override string ToString()
        //{
        //    return KeyValuePair.PairToString(Key, Value);
        //}

        /// <summary>
        /// Deconstructs the current <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the current <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <param name="value">The value of the current <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = Key;
            value = Value;
        }
    }
}