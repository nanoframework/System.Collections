// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace System.Collections.Generic
{
    // An IDictionary is a possibly unordered set of key-value pairs.
    // Keys can be any non-null object.  Values can be any object.
    // You can look up a value in an IDictionary via the default indexed
    // property, Items.

    /// <summary>
    /// Represents a generic collection of key/value pairs.
    /// </summary>
    /// <remarks>
    /// The <see cref="IDictionary{TKey, TValue}"/> interface is the base interface for generic collections of key/value pairs.
    /// Each element is a key/value pair stored in a <see cref="IDictionary{TKey, TValue}"/> object.
    /// Each pair must have a unique key. Implementations can vary in whether they allow key to be <see langword="null"/>. The value can be <see langword="null"/> and does not have to be unique. The <see cref="IDictionary{TKey, TValue}"/> interface allows the contained keys and values to be enumerated, but it does not imply any particular sort order.
    /// The foreach statement of the C# language (For Each in Visual Basic) returns an object of the type of the elements in the collection. Since each element of the <see cref="IDictionary{TKey, TValue}"/> is a key/value pair, the element type is not the type of the key or the type of the value. Instead, the element type is <see cref="IDictionary{TKey, TValue}"/>.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>The element with the specified key.</value>
        TValue this[TKey key]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/>{T} containing the keys of the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>An <see cref="ICollection"/>{T} containing the keys of the object that implements <see cref="IDictionary{TKey, TValue}"/>.</value>
        ICollection<TKey> Keys
        {
            get;
        }

        /// <summary>
        /// Gets an  ICollection{T} containing the values in the <see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <value>An ICollection{T} containing the values in the object that implements <see cref="IDictionary{TKey, TValue}"/>.</value>
        ICollection<TValue> Values
        {
            get;
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="IDictionary{TKey, TValue}"/> contains an element with the key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IDictionary{TKey, TValue}"/>.</exception>"
        /// <exception cref="NotSupportedException">The IDictionary{TKey,TValue} is read-only.</exception>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if key was not found in the original <see cref="IDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception >
        /// <exception cref="NotSupportedException">The <see cref="IDictionary{TKey, TValue}"/> is read-only.</exception>"
        bool Remove(TKey key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the object that implements <see cref="IDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    }
}
