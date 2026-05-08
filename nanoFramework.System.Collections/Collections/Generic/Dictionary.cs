// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// Keys must be unique and non-null. Values can be null for reference types.
    /// Key equality is determined by the key's <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/> implementations.
    /// </remarks>
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        // Entries with next >= -1 are in use (next is the chain index, -1 = end of chain).
        // Entries with next < -1 are free (next encodes the next free index via StartOfFreeList encoding).
        private const int StartOfFreeList = -3;

        private int[] _buckets = null!;
        private Entry[] _entries = null!;
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private KeyCollection? _keys;
        private ValueCollection? _values;

        private struct Entry
        {
            public uint hashCode;
            public int next;        // see class-level comment above
            public TKey key;
            public TValue value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public Dictionary()
        {
            Initialize(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="Dictionary{TKey, TValue}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public Dictionary(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Initialize(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that contains elements copied from the specified <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        public Dictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException();
            }

            Initialize(dictionary.Count);

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public int Count => _count - _freeCount;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </summary>
        /// <value>Always <see langword="false"/>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a collection containing the keys of the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public KeyCollection Keys => _keys ??= new KeyCollection(this);

        /// <summary>
        /// Gets a collection containing the values of the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public ValueCollection Values => _values ??= new ValueCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>The value associated with the specified key.</value>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The property is retrieved and <paramref name="key"/> does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);

                if (i >= 0)
                {
                    return _entries[i].value;
                }

                throw new InvalidOperationException();
            }

            set => TryInsert(key, value, InsertionBehavior.OverwriteExisting);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="Dictionary{TKey, TValue}"/>.</exception>
        public void Add(TKey key, TValue value) => TryInsert(key, value, InsertionBehavior.ThrowOnExisting);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (_buckets.Length > 0)
            {
                uint hashCode = (uint)key.GetHashCode();
                int bucket = (int)(hashCode % (uint)_buckets.Length);
                int last = -1;
                int i = _buckets[bucket];

                while (i >= 0)
                {
                    if (_entries[i].hashCode == hashCode && _entries[i].key.Equals(key))
                    {
                        if (last < 0)
                        {
                            _buckets[bucket] = _entries[i].next;
                        }
                        else
                        {
                            _entries[last].next = _entries[i].next;
                        }

                        _entries[i].next = StartOfFreeList - _freeList;
                        _entries[i].key = default!;
                        _entries[i].value = default!;
                        _freeList = i;
                        _freeCount++;
                        _version++;
                        return true;
                    }

                    last = i;
                    i = _entries[i].next;
                }
            }

            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int i = FindEntry(item.Key);

            if (i >= 0 && EqualityHelper.Equals(_entries[i].value, item.Value))
            {
                Remove(item.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Dictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool ContainsKey(TKey key) => FindEntry(key) >= 0;

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="Dictionary{TKey, TValue}"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if the <see cref="Dictionary{TKey, TValue}"/> contains an element with the specified value; otherwise, <see langword="false"/>.</returns>
        public bool ContainsValue(TValue value)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].next >= -1 && EqualityHelper.Equals(_entries[i].value, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><see langword="true"/> if the <see cref="Dictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int i = FindEntry(key);

            if (i >= 0)
            {
                value = _entries[i].value;
                return true;
            }

            value = default!;
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            int i = FindEntry(item.Key);
            return i >= 0 && EqualityHelper.Equals(_entries[i].value, item.Value);
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            int count = _count;

            if (count > 0)
            {
                for (int i = 0; i < _buckets.Length; i++)
                {
                    _buckets[i] = -1;
                }

                Array.Clear(_entries, 0, count);
                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                _version++;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="Dictionary{TKey, TValue}"/> to the specified array of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source is greater than the available space.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if ((uint)index > (uint)array.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].next >= -1)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(_entries[i].key, _entries[i].value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}.Enumerator"/> for the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int FindEntry(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (_buckets.Length > 0)
            {
                uint hashCode = (uint)key.GetHashCode();
                int i = _buckets[hashCode % (uint)_buckets.Length];

                while (i >= 0)
                {
                    if (_entries[i].hashCode == hashCode && _entries[i].key.Equals(key))
                    {
                        return i;
                    }

                    i = _entries[i].next;
                }
            }

            return -1;
        }

        private enum InsertionBehavior
        {
            None = 0,
            OverwriteExisting = 1,
            ThrowOnExisting = 2,
        }

        private void TryInsert(TKey key, TValue value, InsertionBehavior behavior)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            uint hashCode = (uint)key.GetHashCode();
            int targetBucket = (int)(hashCode % (uint)_buckets.Length);

            for (int i = _buckets[targetBucket]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _entries[i].key.Equals(key))
                {
                    if (behavior == InsertionBehavior.OverwriteExisting)
                    {
                        _entries[i].value = value;
                        _version++;
                        return;
                    }

                    if (behavior == InsertionBehavior.ThrowOnExisting)
                    {
                        throw new ArgumentException();
                    }

                    return;
                }
            }

            int index;

            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = StartOfFreeList - _entries[index].next;
                _freeCount--;
            }
            else
            {
                if (_count == _entries.Length)
                {
                    Resize();
                    targetBucket = (int)(hashCode % (uint)_buckets.Length);
                }

                index = _count;
                _count++;
            }

            _entries[index].hashCode = hashCode;
            _entries[index].next = _buckets[targetBucket];
            _entries[index].key = key;
            _entries[index].value = value;
            _buckets[targetBucket] = index;
            _version++;
        }

        private void Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            _buckets = new int[size];

            for (int i = 0; i < size; i++)
            {
                _buckets[i] = -1;
            }

            _entries = new Entry[size];
            _freeList = -1;
        }

        private void Resize()
        {
            int newSize = HashHelpers.ExpandPrime(_count);
            int[] newBuckets = new int[newSize];

            for (int i = 0; i < newSize; i++)
            {
                newBuckets[i] = -1;
            }

            Entry[] newEntries = new Entry[newSize];
            Array.Copy(_entries, newEntries, _count);

            // Relink all active entries into the new bucket layout
            for (int i = 0; i < _count; i++)
            {
                if (newEntries[i].next >= -1)
                {
                    int bucket = (int)(newEntries[i].hashCode % (uint)newSize);
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;
            private readonly int _version;
            private int _index;
            private KeyValuePair<TKey, TValue> _current;

            internal Enumerator(Dictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="Dictionary{TKey, TValue}"/>.
            /// </summary>
            /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                while ((uint)_index < (uint)_dictionary._count)
                {
                    if (_dictionary._entries[_index].next >= -1)
                    {
                        _current = new KeyValuePair<TKey, TValue>(
                            _dictionary._entries[_index].key,
                            _dictionary._entries[_index].value);
                        _index++;
                        return true;
                    }

                    _index++;
                }

                _index = _dictionary._count + 1;
                _current = default;
                return false;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current => _current;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Dictionary{TKey, TValue}.Enumerator"/>.
            /// </summary>
            public void Dispose() { }
        }

        /// <summary>
        /// Represents the collection of keys in a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class that reflects the keys in the specified <see cref="Dictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> whose keys are reflected in the new <see cref="KeyCollection"/>.</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
            public KeyCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException();
                }

                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="KeyCollection"/>.
            /// </summary>
            public int Count => _dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection<TKey>.Contains(TKey item) => _dictionary.ContainsKey(item);

            /// <summary>
            /// Copies the <see cref="KeyCollection"/> elements to an existing one-dimensional array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The destination array.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">The number of elements is greater than the available space.</exception>
            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException();
                }

                if ((uint)index > (uint)array.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    throw new ArgumentException();
                }

                int count = _dictionary._count;

                for (int i = 0; i < count; i++)
                {
                    if (_dictionary._entries[i].next >= -1)
                    {
                        array[index++] = _dictionary._entries[i].key;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
            /// </summary>
            public KeyCollectionEnumerator GetEnumerator() => new KeyCollectionEnumerator(_dictionary);

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

            void ICollection<TKey>.Clear() => throw new NotSupportedException();

            bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();
        }

        /// <summary>
        /// Represents the collection of values in a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class that reflects the values in the specified <see cref="Dictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> whose values are reflected in the new <see cref="ValueCollection"/>.</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
            public ValueCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException();
                }

                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="ValueCollection"/>.
            /// </summary>
            public int Count => _dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection<TValue>.Contains(TValue item) => _dictionary.ContainsValue(item);

            /// <summary>
            /// Copies the <see cref="ValueCollection"/> elements to an existing one-dimensional array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The destination array.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">The number of elements is greater than the available space.</exception>
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException();
                }

                if ((uint)index > (uint)array.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    throw new ArgumentException();
                }

                int count = _dictionary._count;

                for (int i = 0; i < count; i++)
                {
                    if (_dictionary._entries[i].next >= -1)
                    {
                        array[index++] = _dictionary._entries[i].value;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="ValueCollection"/>.
            /// </summary>
            public ValueCollectionEnumerator GetEnumerator() => new ValueCollectionEnumerator(_dictionary);

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

            void ICollection<TValue>.Clear() => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="KeyCollection"/>.
        /// </summary>
        public struct KeyCollectionEnumerator : IEnumerator<TKey>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;
            private readonly int _version;
            private int _index;
            private TKey? _current;

            internal KeyCollectionEnumerator(Dictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Advances the enumerator to the next element.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                while ((uint)_index < (uint)_dictionary._count)
                {
                    if (_dictionary._entries[_index].next >= -1)
                    {
                        _current = _dictionary._entries[_index].key;
                        _index++;
                        return true;
                    }

                    _index++;
                }

                _index = _dictionary._count + 1;
                _current = default;
                return false;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public TKey Current => _current!;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current!;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="KeyCollectionEnumerator"/>.
            /// </summary>
            public void Dispose() { }
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="ValueCollection"/>.
        /// </summary>
        public struct ValueCollectionEnumerator : IEnumerator<TValue>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;
            private readonly int _version;
            private int _index;
            private TValue? _current;

            internal ValueCollectionEnumerator(Dictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Advances the enumerator to the next element.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                while ((uint)_index < (uint)_dictionary._count)
                {
                    if (_dictionary._entries[_index].next >= -1)
                    {
                        _current = _dictionary._entries[_index].value;
                        _index++;
                        return true;
                    }

                    _index++;
                }

                _index = _dictionary._count + 1;
                _current = default;
                return false;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public TValue Current => _current!;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current!;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException();
                }

                _index = 0;
                _current = default;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="ValueCollectionEnumerator"/>.
            /// </summary>
            public void Dispose() { }
        }
    }

    internal static class HashHelpers
    {
        private static readonly int[] s_primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353,
            431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861,
            5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627,
            52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371,
        };

        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < s_primes.Length; i++)
            {
                if (s_primes[i] >= min)
                {
                    return s_primes[i];
                }
            }

            // Fallback: search for an odd number that is prime
            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i))
                {
                    return i;
                }
            }

            return min;
        }

        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            if ((uint)newSize > 0x7FEFFFFD && 0x7FEFFFFD > oldSize)
            {
                return 0x7FEFFFFD;
            }

            return GetPrime(newSize);
        }

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                // Compute integer square root without Math.Sqrt
                int limit = candidate;
                int root = 1;
                while (root * root <= limit)
                {
                    root++;
                }

                root--; // root is now floor(sqrt(candidate))

                for (int divisor = 3; divisor <= root; divisor += 2)
                {
                    if (candidate % divisor == 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return candidate == 2;
        }
    }

    internal static class EqualityHelper
    {
        public static bool Equals<T>(T x, T y)
        {
            if (x == null)
            {
                return y == null;
            }

            return x.Equals(y);
        }
    }
}
