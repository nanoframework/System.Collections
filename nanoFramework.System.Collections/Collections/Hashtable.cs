//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Hashtable"/> class.
    /// </summary>
    /// <remarks>
    /// The implementation for .NET nanoFramework, unlike the full .NET, doesn't support collisions so every key has to be truly unique through it's <see cref="object.GetHashCode"/>.
    /// </remarks>
    [Serializable]
    public class Hashtable : ICloneable, IDictionary
    {
        private const int InitialSize = 3;
        private const float DefaultLoadFactor = 1.0f;

        private readonly Bucket[] _buckets;
        private readonly int _loadsize;
        private float _loadFactor;
        private int _count;
        private int _version;

        // this is used as the lock object 
        // a lock is required because multiple threads can access the Hashtable
        private static readonly object _syncLock = new();

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable"/> class using the default initial capacity, load factor, hash code provider and comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A hash table's capacity is used to calculate the optimal number of hash table buckets based on the load factor. Capacity is automatically increased as required.
        /// </para>
        /// <para>
        /// The load factor is the maximum ratio of elements to buckets. A smaller load factor means faster lookup at the cost of increased memory consumption.
        /// </para>
        /// <para>
        /// When the actual load factor reaches the specified load factor, the number of buckets is automatically increased to the smallest prime number that is larger than twice the current number of buckets.
        /// </para>
        /// <para>
        /// The hash code provider dispenses hash codes for keys in the <see cref="Hashtable"/> object. The default hash code provider is the key's implementation of <see cref="object.GetHashCode"/>.
        /// </para>
        /// The .NET nanoFramework implementation uses the <see cref="object.Equals(object)"/> to perform comparisons of the key's.
        /// </remarks>
        public Hashtable() : this(InitialSize, DefaultLoadFactor) { }

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable"/> class using the specified initial capacity and load factor, and the default hash code provider and comparer.
        /// </summary>
        /// <param name="capacity">The approximate number of elements that the <see cref="Hashtable"/> object can initially contain.</param>
        /// <exception cref="ArgumentException"><paramref name="capacity"/> is causing an overflow.</exception>
        /// <remarks>
        /// <para>
        /// Specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="Hashtable"/> object. Capacity is automatically increased as required based on the load factor.
        /// </para>
        /// <para>
        /// The load factor is the maximum ratio of elements to buckets. A smaller load factor means faster lookup at the cost of increased memory consumption.
        /// </para>
        /// <para>
        /// When the actual load factor reaches the specified load factor, the number of buckets is automatically increased to the smallest prime number that is larger than twice the current number of buckets.
        /// </para>
        /// <para>
        /// The hash code provider dispenses hash codes for keys in the <see cref="Hashtable"/> object. The default hash code provider is the key's implementation of <see cref="object.GetHashCode"/>.
        /// </para>
        /// The .NET nanoFramework implementation uses the <see cref="object.Equals(object)"/> to perform comparisons of the key's.
        /// </remarks>
        public Hashtable(int capacity) : this(capacity, DefaultLoadFactor) { }

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable"/> class using the specified initial capacity and load factor, and the default hash code provider and comparer.
        /// load factor.
        /// </summary>
        /// <param name="capacity">The approximate number of elements that the <see cref="Hashtable"/> object can initially contain.</param>
        /// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>
        /// <paramref name="capacity"/> is less than zero.
        /// </para>
        /// <para>
        /// -or>
        /// </para>
        /// <para>
        /// <paramref name="loadFactor"/> is less than 0.1.
        /// </para>
        /// <para>
        /// -or>
        /// </para>
        /// <para>
        /// <paramref name="loadFactor"/> is greater than 1.0.
        /// </para>
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="capacity"/> is causing an overflow.</exception>
        /// <remarks>
        /// <para>
        /// Specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="Hashtable"/> object. Capacity is automatically increased as required based on the load factor.
        /// </para>
        /// <para>
        /// The load factor is the maximum ratio of elements to buckets. A smaller load factor means faster lookup at the cost of increased memory consumption.
        /// </para>
        /// <para>
        /// When the actual load factor reaches the specified load factor, the number of buckets is automatically increased to the smallest prime number that is larger than twice the current number of buckets.
        /// </para>
        /// <para>
        /// The hash code provider dispenses hash codes for keys in the <see cref="Hashtable"/> object. The default hash code provider is the key's implementation of <see cref="object.GetHashCode"/>.
        /// </para>
        /// The .NET nanoFramework implementation uses the <see cref="object.Equals(object)"/> to perform comparisons of the key's.
        /// </remarks>
        public Hashtable(int capacity, float loadFactor)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!(loadFactor >= 0.1f && loadFactor <= 1.0f))
            {
                throw new ArgumentOutOfRangeException();
            }

            // from full .NET seems that .72 is the optimal load factor
            _loadFactor = 0.72f * loadFactor;

            double rawsize = capacity / _loadFactor;
            if (rawsize > int.MaxValue)
            {
                throw new ArgumentException();
            }

            // avoid awfully small sizes
            int hashsize = (rawsize > InitialSize) ? GetPrimeNative((int)rawsize) : InitialSize;
            _buckets = new Bucket[hashsize];

            _loadsize = (int)(_loadFactor * hashsize);

            // Based on the current algorithm, loadsize must be less than hashsize.
            Debug.Assert(_loadsize < hashsize, "Invalid hashtable loadsize!");
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a shallow copy of the <see cref="Hashtable"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="Hashtable"/>.</returns>
        /// <remarks>
        /// <para>
        /// A shallow copy of a collection copies only the elements of the collection, whether they are reference types or value types, but it does not copy the objects that the references refer to. The references in the new collection point to the same objects that the references in the original collection point to.
        /// </para>
        /// <para>
        /// In contrast, a deep copy of a collection copies the elements and everything directly or indirectly referenced by the elements.
        /// </para>
        /// <para>
        /// The <see cref="Hashtable"/> clone has the same count, the same capacity, the same hash provider, and the same comparer as the original <see cref="Hashtable"/>.
        /// </para>
        /// </remarks>
        public object Clone()
        {
            lock (_syncLock)
            {
                Hashtable ht = new(_buckets.Length)
                {
                    _count = 0,
                    _loadFactor = _loadFactor,
                    _version = _version
                };

                int bucket = _buckets.Length;

                while (bucket > 0)
                {
                    bucket--;

                    // can only copy buckets that aren't null
                    if (_buckets[bucket] != null)
                    {
                        object keyv = _buckets[bucket]._key;

                        if (keyv != null)
                        {
                            ht[keyv] = _buckets[bucket]._value;
                        }
                    }
                }

                return ht;
            }
        }

        #endregion ICloneable Members

        #region IEnumerable Members

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that iterates through the Hashtable.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="Hashtable"/>.</returns>
        /// <remarks>
        /// <para>
        /// The <see langword="foreach"/> statement of the C# language (for each in Visual Basic) hides the complexity of the enumerators. Therefore, using <see langword="foreach"/> is recommended, instead of directly manipulating the enumerator.
        /// </para>
        /// <para>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// </para>
        /// <para>
        /// Initially, the enumerator is positioned before the first element in the collection. <see cref="IEnumerator.Reset"/> also brings the enumerator back to this position. At this position, <see cref="IEnumerator.Current"/> is undefined. Therefore, you must call <see cref="IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="IEnumerator.Current"/>.
        /// </para>
        /// <para>
        /// <see cref="IEnumerator.Current"/> returns the same object until either <see cref="IEnumerator.MoveNext"/> or <see cref="IEnumerator.Reset"/> is called. MoveNext sets Current to the next element.
        /// </para>
        /// <para>
        /// If <see cref="IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="IEnumerator.MoveNext"/> returns false. When the enumerator is at this position, subsequent calls to <see cref="IEnumerator.MoveNext"/> also return false. If the last call to <see cref="IEnumerator.MoveNext"/> returned false, <see cref="IEnumerator.Current"/> is undefined. To set <see cref="IEnumerator.Current"/> to the first element of the collection again, you can call <see cref="IEnumerator.Reset"/> followed by <see cref="IEnumerator.MoveNext"/>.
        /// </para>
        /// <para>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and its behavior is undefined.
        /// </para>
        /// <para>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration. To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// </para>
        /// </remarks>
        public IEnumerator GetEnumerator() => new HashtableEnumerator(this, HashtableEnumerator.DictEntry);

        #endregion IEnumerable Members

        #region ICollection Members

        /// <summary>
        /// Gets the number of elements contained in the ICollection.
        /// </summary>
        /// <value>
        /// The number of elements contained in the ICollection.
        /// </value>
        public int Count => _count;

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        /// <value>
        /// true if access to the ICollection is synchronized (thread safe); otherwise, false.
        /// </value>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the ICollection.
        /// </value>
        public object SyncRoot => this;

        /// <summary>
        /// Copies the <see cref="Hashtable"/> elements to a one-dimensional <see cref="Array"/> instance at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the <see cref="DictionaryEntry"/> objects copied from <see cref="Hashtable"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="Hashtable"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        /// <remarks>
        /// <para>
        /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates through the <see cref="Hashtable"/>.
        /// </para>
        /// </remarks>
        public void CopyTo(
            Array array,
            int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (arrayIndex < 0 && arrayIndex > _buckets.Length)
            {
#pragma warning disable S112 // General exceptions should never be thrown
                throw new ArgumentOutOfRangeException();
#pragma warning restore S112 // General exceptions should never be thrown
            }

            if (array.Length - arrayIndex < Count)
            {
#pragma warning disable S112 // General exceptions should never be thrown
                throw new ArgumentException();
#pragma warning restore S112 // General exceptions should never be thrown
            }

            lock (_syncLock)
            {
                int j = 0;

                for (int i = _buckets.Length; --i >= 0;)
                {
                    object keyv = _buckets[i]._key;

                    if ((keyv != null) && (keyv != _buckets))
                    {
                        ((IList)array)[j] = new DictionaryEntry(_buckets[i]._key, _buckets[i]._value);
                        j++;
                    }
                }
            }
        }

        #endregion ICollection Members

        #region IDictionary Members

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public bool IsFixedSize => false;

        /// <inheritdoc/>
        public ICollection Keys => new KeyCollection(this);

        /// <inheritdoc/>
        public ICollection Values => new ValueCollection(this);

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>The element with the specified key, or <see langword="null"/> if the key does not exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>"
        public object this[object key]
        {
            get
            {

                if (key == null)
                {
                    throw new ArgumentNullException();
                }

                return GetNative(
                    key,
                    key.GetHashCode());
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException();
                }

                InsertNative(
                    key,
                    value,
                    false,
                    key.GetHashCode());
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="Hashtable"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="Hashtable"/>.</exception>
        public void Add(
            object key,
            object value) => InsertNative(
                key,
                value,
                true,
                key.GetHashCode());

        /// <summary>
        /// Removes all elements from the <see cref="Hashtable"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Count"/> is set to zero, and references to other objects from elements of the collection are also released. The capacity remains unchanged.
        /// </remarks>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Clear();

        /// <summary>
        /// Determines whether the <see cref="Hashtable"/> contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Hashtable"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Hashtable"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <para>
        /// <see cref="Contains"/> implements <see cref="IDictionary.Contains"/>.
        /// </para>
        /// <para>
        /// This method uses the collection's objects' <see cref="object.Equals(object)"/> method on item to determine whether item exists.
        /// </para>
        /// </remarks>
        public bool Contains(object key)
        {
            return ContainsNative(
                key,
                key.GetHashCode());
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Hashtable"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public void Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            RemoveNative(
                key,
                key.GetHashCode());
        }


        #endregion IDictionary Members

        // Implements an enumerator for a hashtable. The enumerator uses the
        // internal version number of the hashtable to ensure that no modifications
        // are made to the hashtable while an enumeration is in progress.
        private sealed class HashtableEnumerator : IDictionaryEnumerator, IEnumerator
        {
            private readonly Hashtable _hashtable;
            private int _bucket;
            private readonly int _version;
            private bool _current;
            private readonly int _getObjectRetType;
            private object _currentKey;
            private object _currentValue;

            internal const int Keys = 1;
            internal const int Values = 2;
            internal const int DictEntry = 3;

            internal HashtableEnumerator(Hashtable hashtable, int getObjRetType)
            {
                _hashtable = hashtable;
                _bucket = hashtable._buckets.Length;
                _version = hashtable._version;
                _current = false;
                _getObjectRetType = getObjRetType;
            }

            public object Clone() => MemberwiseClone();

            public object Key
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException();
                    }

                    return _currentKey!;
                }
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern bool MoveNext();

            public DictionaryEntry Entry
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException();
                    }

                    return new DictionaryEntry(_currentKey!, _currentValue);
                }
            }

            public object Current
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException();
                    }

                    if (_getObjectRetType == Keys)
                    {
                        return _currentKey;
                    }
                    else if (_getObjectRetType == Values)
                    {
                        return _currentValue;
                    }
                    else
                    {
                        return new DictionaryEntry(_currentKey, _currentValue);
                    }
                }
            }

            public object Value
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException();
                    }

                    return _currentValue;
                }
            }

            public void Reset()
            {
                if (_version != _hashtable._version)
                {
                    throw new InvalidOperationException();
                }

                _current = false;
                _bucket = _hashtable._buckets.Length;
                _currentKey = null;
                _currentValue = null;
            }
        }

        // Implements a Collection for the keys of a hashtable. An instance of this
        // class is created by the GetKeys method of a hashtable.
        private sealed class KeyCollection : ICollection
        {
            private readonly Hashtable _hashtable;

            internal KeyCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                if (array is null)
                {
                    throw new ArgumentNullException();
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (array.Length - arrayIndex < _hashtable._count)
                {
                    throw new ArgumentException();
                }

                _hashtable.CopyKeys(array, arrayIndex);
            }

            public IEnumerator GetEnumerator() => new HashtableEnumerator(_hashtable, HashtableEnumerator.Keys);

            public bool IsSynchronized => _hashtable.IsSynchronized;

            public object SyncRoot => _hashtable.SyncRoot;

            public int Count => _hashtable._count;
        }

        // Implements a Collection for the values of a hashtable. An instance of
        // this class is created by the GetValues method of a hashtable.
        private sealed class ValueCollection : ICollection
        {
            private readonly Hashtable _hashtable;

            internal ValueCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                if (array is null)
                {
                    throw new ArgumentNullException();
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (array.Length - arrayIndex < _hashtable._count)
                {
                    throw new ArgumentException();
                }

                _hashtable.CopyValues(array, arrayIndex);
            }

            public IEnumerator GetEnumerator()
            {
                return new HashtableEnumerator(_hashtable, HashtableEnumerator.Values);
            }

            public bool IsSynchronized => _hashtable.IsSynchronized;

            public object SyncRoot => _hashtable.SyncRoot;

            public int Count => _hashtable._count;
        }

        #region helper methods

        // Copies the keys of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the KeyCollection class.
        private void CopyKeys(Array array, int arrayIndex)
        {
            Debug.Assert(array != null);

            lock (_syncLock)
            {
                int j = 0;

                for (int i = _buckets.Length; --i >= 0;)
                {
                    // can only work with buckets that aren't null
                    if (_buckets[i] != null)
                    {
                        object keyv = _buckets[i]._key;

                        if ((keyv != null) && (keyv != _buckets))
                        {
                            ((IList)array)[arrayIndex + j] = keyv;
                            j++;
                        }
                    }
                }
            }
        }

        // Copies the values of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the ValueCollection class.
        private void CopyValues(Array array, int arrayIndex)
        {
            Debug.Assert(array != null);

            lock (_syncLock)
            {
                int j = 0;

                for (int i = _buckets.Length; --i >= 0;)
                {
                    object keyv = _buckets[i]._key;

                    if ((keyv != null) && (keyv != _buckets))
                    {
                        ((IList)array)[arrayIndex + j] = _buckets[i]._value;
                        j++;
                    }
                }
            }
        }

        #endregion

        #region Native methods

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InsertNative(
           object key,
           object newValue,
           bool add,
           int keyHashCode);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern object GetNative(
            object key,
            int keyHashCode);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static int GetPrimeNative(int min);


        [MethodImpl(MethodImplOptions.InternalCall)]

        private extern bool ContainsNative(
            object key,
            int keyHashCode);


        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void RemoveNative(
            object key,
            int keyHashCode);

        #endregion
    }
}
