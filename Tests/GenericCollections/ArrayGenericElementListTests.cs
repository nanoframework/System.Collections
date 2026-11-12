// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;
using System;
using System.Collections.Generic;

namespace GenericCollections
{
    [TestClass]
    public class ArrayGenericElementListTests
    {
        [TestMethod]
        public void IList_OverGenericInstanceArray_ComparesTheArguments()
        {
            object array = new ListBox<int>[2];

            Assert.IsTrue(array is IList<ListBox<int>>);
            Assert.IsFalse(array is IList<ListBox<string>>);
            Assert.ThrowsException(typeof(InvalidCastException), () => { _ = (IList<ListBox<string>>)array; });
        }

        [TestMethod]
        public void IList_OverGenericInstanceArray_IndexerAndIndexOf()
        {
            ListBox<int> first = new ListBox<int>();
            ListBox<int> second = new ListBox<int>();
            ListBox<int>[] array = new ListBox<int>[] { first, null };
            IList<ListBox<int>> list = (IList<ListBox<int>>)(object)array;

            Assert.IsTrue(object.ReferenceEquals(list[0], first));
            Assert.AreEqual(0, list.IndexOf(first));
            Assert.AreEqual(-1, list.IndexOf(second));

            list[1] = second;
            Assert.IsTrue(object.ReferenceEquals(array[1], second));
            Assert.AreEqual(1, list.IndexOf(second));
        }

        [TestMethod]
        public void IList_OverGenericInstanceArray_RejectsAnotherInstantiationOnStore()
        {
            object[] array = new ListStoreBox<int>[1];

            Assert.ThrowsException(typeof(Exception), () => { array[0] = new ListStoreBox<string>(); });
        }

        [TestMethod]
        public void IList_OverGenericInstanceArray_StructuralChangesThrow()
        {
            IList<ListBox<int>> list = (IList<ListBox<int>>)(object)new ListBox<int>[1];

            Assert.ThrowsException(typeof(NotSupportedException), () => { list.Insert(0, new ListBox<int>()); });
            Assert.ThrowsException(typeof(NotSupportedException), () => { list.RemoveAt(0); });
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => { _ = list[1]; });
        }

        [TestMethod]
        public void IList_OverJaggedArray_IndexerAndIndexOf()
        {
            int[] row = new int[] { 1, 2 };
            int[][] array = new int[][] { new int[1], row };
            IList<int[]> list = (IList<int[]>)(object)array;

            Assert.IsTrue(object.ReferenceEquals(list[1], row));
            Assert.AreEqual(1, list.IndexOf(row));
            Assert.AreEqual(-1, list.IndexOf(new int[1]));
            Assert.IsTrue(list.Contains(row));
            Assert.IsFalse((object)array is IList<string[]>);
        }

        [TestMethod]
        public void IList_OverJaggedGenericInstanceArray_ComparesTheArguments()
        {
            ListJaggedBox<int>[][] array = new ListJaggedBox<int>[2][];
            array[1] = new ListJaggedBox<int>[1];
            IList<ListJaggedBox<int>[]> list = (IList<ListJaggedBox<int>[]>)(object)array;

            Assert.IsTrue(object.ReferenceEquals(list[1], array[1]));
            Assert.AreEqual(1, list.IndexOf(array[1]));
            Assert.IsFalse((object)array is IList<ListJaggedBox<string>[]>);
        }

        [TestMethod]
        public void IList_OverStructArray_ReadWrite()
        {
            ListStruct<int>[] array = new ListStruct<int>[2];
            IList<ListStruct<int>> list = (IList<ListStruct<int>>)(object)array;

            Assert.AreEqual(0, list[0].Value);

            ListStruct<int> element = new ListStruct<int>();
            element.Value = 5;
            list[1] = element;

            Assert.AreEqual(5, array[1].Value);
            Assert.IsFalse((object)array is IList<ListStruct<string>>);
        }

        [TestMethod]
        public void IList_OverArrayCreatedInGenericCode()
        {
            object plain = new ListHolder<ListGenericBox<int>>().Make();
            object jagged = new ListHolder<ListGenericBox<int>[]>().Make();

            Assert.IsTrue(plain is IList<ListGenericBox<int>>);
            Assert.IsFalse(plain is IList<ListGenericBox<string>>);
            Assert.IsTrue(jagged is IList<ListGenericBox<int>[]>);
            Assert.IsFalse(jagged is IList<ListGenericBox<string>[]>);
        }
    }

    public class ListBox<T> { }
    public class ListStoreBox<T> { }
    public class ListJaggedBox<T> { }
    public class ListGenericBox<T> { }
    public struct ListStruct<T> { public int Value; }
    public class ListHolder<T> { public T[] Make() { return new T[2]; } }
}
