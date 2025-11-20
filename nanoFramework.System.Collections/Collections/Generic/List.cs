// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class List<T> : IList<T>, IList, IReadOnlyList<T>
    {
        private const int DefaultCapacity = 4;

        internal T[] _items;
        internal int _size;
        internal int _version;

#pragma warning disable CA1825, IDE0300 // avoid the extra generic instantiation for Array.Empty<T>()
        private static readonly T[] s_emptyArray = new T[0];
#pragma warning restore CA1825, IDE0300

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public List()
        {
            _items = s_emptyArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public List(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (capacity == 0)
            {
                _items = s_emptyArray;
            }
            else
            {
                _items = new T[capacity];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public List(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException();
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;

                if (count == 0)
                {
                    _items = s_emptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _items = s_emptyArray;

                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        /// The number of elements that the <see cref="List{T}"/> can contain before resizing is required.
        /// </value>
        public int Capacity
        {
            get => _items.Length;

            set
            {
                if (value < _size)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];

                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }

                        _items = newItems;
                    }
                    else
                    {
                        _items = s_emptyArray;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="List{T}"/>.
        /// </summary>
        /// <value>
        /// The number of elements contained in the <see cref="List{T}"/>.
        /// </value>
        public int Count => _size;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IList"/> has a fixed size.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="IList"/> has a fixed size; otherwise, <see langword="false"/>. In the default implementation of <see cref="List{T}"/>, this property always returns false.
        /// </value>
        bool IList.IsFixedSize => false;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="ICollection{T}"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="List{T}"/>, this property always returns false.
        /// </value>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IList"/> is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="IList"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="List{T}"/>, this property always returns <see langword="false"/>.
        /// </value>
        bool IList.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value>
        /// <see langword="true"/> if access to the <see cref="ICollection"/> is synchronized (thread safe); otherwise, <see langword="false"/>. In the default implementation of <see cref="List{T}"/>, this property always returns <see langword="false"/>.
        /// </value>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="ICollection"/>. In the default implementation of <see cref="List{T}"/>, this property always returns the current instance.
        /// </value>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <value>
        /// The element at the specified index.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>
        /// <paramref name="index"/> is less than 0.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para>
        /// </exception>
        public T this[int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)_size)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return _items[index];
            }

            set
            {
                if ((uint)index >= (uint)_size)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _items[index] = value;
                _version++;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <value>
        /// The element at the specified index.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="IList"/>.</exception>
        /// <exception cref="ArgumentException"> The property is set and the value is of a type that isn't assignable to the IList.</exception>
        object? IList.this[int index]
        {
            get => this[index];

            set
            {
                try
                {
                    this[index] = (T)value!;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="List{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        public void Add(T item)
        {
            _version++;

            T[] array = _items;
            int size = _size;

            if ((uint)size < (uint)array.Length)
            {
                _size = size + 1;
                array[size] = item;
            }
            else
            {
                AddWithResize(item);
            }
        }

        private void AddWithResize(T item)
        {
            Debug.Assert(_size == _items.Length);

            int size = _size;

            Grow(size + 1);

            _size = size + 1;
            _items[size] = item;
        }

        /// <summary>
        /// Adds an item to the <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IList"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The index at which the new element was inserted.</returns>
        /// <exception cref="ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="IList"/>.</exception>
        int IList.Add(object item)
        {
            try
            {
                Add((T)item!);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }

            return Count - 1;
        }

        /// <summary>
        /// Removes all elements from the <see cref="List{T}"/>.
        /// </summary>
        public void Clear()
        {
            _version++;

            // Only clear the array if T is a reference type
            if (!typeof(T).IsValueType)
            {
                int size = _size;
                _size = 0;

                if (size > 0)
                {
                    Array.Clear(_items, 0, size);
                }
            }
            else
            {
                _size = 0;
            }
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="List{T}"/> otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item)
        {
            // PERF: IndexOf calls Array.IndexOf, which internally
            // calls EqualityComparer<T>.Default.IndexOf, which
            // is specialized for different types. This
            // boosts performance since instead of making a
            // virtual method call each iteration of the loop,
            // via EqualityComparer<T>.Default.Equals, we
            // only make one virtual call to EqualityComparer.IndexOf.

            return _size != 0 && IndexOf(item) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="IList"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="IList"/> otherwise, <see langword="false"/>.</returns>
        bool IList.Contains(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((T)item!);
            }

            return false;
        }

        /// <summary>
        /// Copies the entire <see cref="List{T}"/> to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        public void CopyTo(T[] array) => CopyTo(array, 0);

        /// <summary>
        /// Copies a range of elements from the <see cref="List{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="List{T}"/> at which copying begins.</param>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">
        /// <para>
        /// index is equal to or greater than the Count of the source <see cref="List{T}"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// The number of elements in the source <see cref="List{T}"/> from index to the end of the <see cref="List{T}"/> is less than count.
        /// </para>
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (_size - index < count)
            {
                throw new ArgumentException();
            }

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the entire <see cref="List{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"> The number of elements in the source <see cref="List{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="List{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            try
            {
                // Array.Copy will check for NULL.
                Array.Copy(_items, 0, array!, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}"/> until all elements have been processed or the action throws an exception.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="List{T}"/> was modified during the operation.</exception>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }

            int version = _version;

            for (int i = 0; i < _size; i++)
            {
                if (version != _version)
                {
                    break;
                }

                action(_items[i]);
            }

            if (version != _version)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="List{T}"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="List{T}"/>.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="List{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="List{T}"/>, if found; otherwise, -1.</returns>
        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _size);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="IList"/>, if found; otherwise, -1.</returns>
        int IList.IndexOf(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item!);
            }
            return -1;
        }

        /// <summary>
        /// Inserts an element into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
        public void Insert(int index, T item)
        {
            // Note that insertions at the end are legal.
            if ((uint)index > (uint)_size)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (_size == _items.Length)
            {
                GrowForInsertion(index, 1);
            }
            else if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }

            _items[index] = item;
            _size++;
            _version++;
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/> and <typeparamref name="T"/> is a value type.</exception>
        /// <exception cref="ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="IList"/>.</exception>
        void IList.Insert(int index, object? item)
        {
            if (default(T) != null && item == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                Insert(index, (T)item!);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="List{T}"/>; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item"/> is not found in the original <see cref="List{T}"/>.</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="IList"/>. The value can be <see langword="null"/> for reference types.</param>
        void IList.Remove(object? item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item!);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. </exception>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException();
            }

            _size--;

            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }

            // TODO
            //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[_size] = default!;
            }

            _version++;
        }

        /// <summary>
        /// Copies the elements of the <see cref="List{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="List{T}"/>.</returns>
        public T[] ToArray()
        {
            if (_size == 0)
            {
                return s_emptyArray;
            }

            T[] array = new T[_size];
            Array.Copy(_items, array, _size);

            return array;
        }

        internal void Grow(int capacity)
        {
            Capacity = GetNewCapacity(capacity);
        }

        internal void GrowForInsertion(int indexToInsert, int insertionCount = 1)
        {
            Debug.Assert(insertionCount > 0);

            int requiredCapacity = checked(_size + insertionCount);
            int newCapacity = GetNewCapacity(requiredCapacity);

            // Inline and adapt logic from set_Capacity

            T[] newItems = new T[newCapacity];
            if (indexToInsert != 0)
            {
                Array.Copy(_items, newItems, length: indexToInsert);
            }

            if (_size != indexToInsert)
            {
                Array.Copy(_items, indexToInsert, newItems, indexToInsert + insertionCount, _size - indexToInsert);
            }

            _items = newItems;
        }

        private int GetNewCapacity(int capacity)
        {
            Debug.Assert(_items.Length < capacity);

            int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

            // Allow the list to grow to maximum possible capacity (limited to the available memory) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > int.MaxValue)
            {
                newCapacity = int.MaxValue;
            }

            // If the computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newCapacity < capacity)
            {
                newCapacity = capacity;
            }

            return newCapacity;
        }

        private static bool IsCompatibleObject(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return (value is T) || (value == null && default(T) == null);
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="List{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly List<T> _list;
            private readonly int _version;

            private int _index;
            private T? _current;

            internal Enumerator(List<T> list)
            {
                _list = list;
                _version = list._version;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="List{T}.Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="List{T}"/>.
            /// </summary>
            /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                List<T> localList = _list;

                if (_version != _list._version)
                {
                    throw new InvalidOperationException();
                }

                if ((uint)_index < (uint)localList._size)
                {
                    _current = localList._items[_index];
                    _index++;
                    return true;
                }

                _current = default;
                _index = -1;
                return false;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>
            /// The element in the <see cref="List{T}"/> at the current position of the enumerator.
            /// </value>
            public T Current => _current!;

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>
            /// The element in the <see cref="List{T}"/> at the current position of the enumerator.
            /// </value>
            object? IEnumerator.Current
            {
                get
                {
                    if (_index <= 0)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the <see cref="List{T}"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            void IEnumerator.Reset()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException();
                }

                _index = 0;
                _current = default;
            }
        }
    }
}
