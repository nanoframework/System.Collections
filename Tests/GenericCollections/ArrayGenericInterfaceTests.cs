// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;
using System;
using System.Collections.Generic;

namespace GenericCollections
{
    [TestClass]
    public class ArrayGenericInterfaceTests
    {
        [TestMethod]
        public void Array_IsInstance_OfGenericCollectionInterfaces()
        {
            object ints = new int[1];

            Assert.IsTrue(ints is IEnumerable<int>);
            Assert.IsTrue(ints is ICollection<int>);
            Assert.IsTrue(ints is IList<int>);
            Assert.IsTrue(ints is IReadOnlyCollection<int>);
            Assert.IsTrue(ints is IReadOnlyList<int>);
        }

        [TestMethod]
        public void Array_IsNotInstance_OfOtherElementType()
        {
            object ints = new int[1];

            Assert.IsFalse(ints is IList<uint>);
            Assert.IsFalse(ints is IList<string>);
            Assert.IsFalse(ints is IList<object>);
            Assert.IsFalse(ints is IReadOnlyCollection<string>);
            Assert.IsFalse(ints is IReadOnlyList<object>);
            Assert.IsNull(ints as IList<string>);
            Assert.IsNotNull(ints as IList<int>);
        }

        [TestMethod]
        public void ReferenceArray_IsCovariant()
        {
            object strings = new string[1];

            Assert.IsTrue(strings is IList<string>);
            Assert.IsTrue(strings is IList<object>);
            Assert.IsTrue(strings is IReadOnlyList<object>);
            Assert.IsFalse(strings is IList<Exception>);
        }

        [TestMethod]
        public void Array_CastToWrongElementType_Throws()
        {
            object ints = new int[1];

            Assert.ThrowsException(typeof(InvalidCastException), () => { _ = (IList<string>)ints; });
            Assert.ThrowsException(typeof(InvalidCastException), () => { _ = (IReadOnlyList<object>)ints; });
        }

        [TestMethod]
        public void IList_OverArray_ReadWrite()
        {
            int[] array = new int[] { 1, 2, 3 };
            IList<int> list = (IList<int>)(object)array;

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list[1]);
            Assert.IsTrue(list.Contains(3));
            Assert.IsFalse(list.Contains(9));
            Assert.AreEqual(2, list.IndexOf(3));
            Assert.IsTrue(list.IsReadOnly);

            list[0] = 10;
            Assert.AreEqual(10, array[0]);
        }

        [TestMethod]
        public void IList_OverArray_StructuralChangesThrow()
        {
            int[] array = new int[] { 1, 2 };
            IList<int> list = (IList<int>)(object)array;

            Assert.ThrowsException(typeof(NotSupportedException), () => { list.Add(3); });
            Assert.ThrowsException(typeof(NotSupportedException), () => { list.Insert(0, 3); });
            Assert.ThrowsException(typeof(NotSupportedException), () => { list.Remove(1); });
            Assert.ThrowsException(typeof(NotSupportedException), () => { list.RemoveAt(0); });
            Assert.ThrowsException(typeof(NotSupportedException), () => { list.Clear(); });

            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
        }

        [TestMethod]
        public void IReadOnlyList_OverArray()
        {
            IReadOnlyList<string> list = (IReadOnlyList<string>)(object)new string[] { "a", "b" };

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("b", list[1]);
        }

        [TestMethod]
        public void IList_OverStringArray_AsObject()
        {
            IList<object> list = (IList<object>)(object)new string[] { "a", "b" };

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("a", list[0]);
        }

        [TestMethod]
        public void List_Constructor_FromArrays()
        {
            var ints = new List<int>(new int[] { 1, 2, 3 });
            Assert.AreEqual(3, ints.Count);
            Assert.AreEqual(3, ints[2]);

            var strings = new List<string>(new string[] { "a", "b" });
            Assert.AreEqual(2, strings.Count);
            Assert.AreEqual("b", strings[1]);

            var objects = new List<object>(new string[] { "a", "b" });
            Assert.AreEqual(2, objects.Count);
            Assert.AreEqual("a", objects[0]);
        }

        [TestMethod]
        public void List_Constructor_FromEmptyArray()
        {
            var list = new List<int>(new int[0]);

            Assert.AreEqual(0, list.Count);
        }
    }
}
