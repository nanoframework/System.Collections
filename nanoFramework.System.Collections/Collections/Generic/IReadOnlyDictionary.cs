// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a generic read-only collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose associated value is to be retrieved.</param>
        /// <returns>The value associated with the specified key if the key is found; otherwise, the default value for the type
        /// of the value.</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the keys in the read-only dictionary.</value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the values in the read-only dictionary.</value>
        IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><see langword="true"/> if the read-only dictionary contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the object that implements the <see cref="IReadOnlyDictionary{TKey,TValue}"/> interface contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method combines the functionality of the <see cref="ContainsKey"/> method and the <see cref="this"/> property.
        /// If the key is not found, the <paramref name="value"/> parameter gets the appropriate default value for the type TValue: for example, 0 (zero) for integer types, <see langword="false"/> for <see cref="bool"/> types, and <see langword="null"/> for reference types.
        /// </remarks>
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    }
}