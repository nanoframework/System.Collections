// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;
using System;
using System.Collections.Generic;

namespace GenericCollections
{
    [TestClass]
    public class VarianceTests
    {
        [TestMethod]
        public void List_IsCovariantReadOnlyView()
        {
            object list = new List<string>();

            Assert.IsTrue(list is IEnumerable<string>);
            Assert.IsTrue(list is IEnumerable<object>);
            Assert.IsTrue(list is IReadOnlyCollection<object>);
            Assert.IsTrue(list is IReadOnlyList<object>);
            Assert.IsTrue(list is IReadOnlyList<string>);
        }

        [TestMethod]
        public void List_IsNotCovariantMutableView()
        {
            object list = new List<string>();

            Assert.IsTrue(list is IList<string>);
            Assert.IsFalse(list is IList<object>);
            Assert.IsFalse(list is ICollection<object>);
            Assert.IsFalse(list is IReadOnlyList<Exception>);
            Assert.IsFalse(list is IEnumerable<VarOther>);
        }

        [TestMethod]
        public void List_OfValueType_IsNotCovariant()
        {
            object list = new List<int>();

            Assert.IsTrue(list is IReadOnlyList<int>);
            Assert.IsFalse(list is IEnumerable<object>);
            Assert.IsFalse(list is IReadOnlyList<uint>);
            Assert.IsFalse(list is IReadOnlyList<long>);
        }

        [TestMethod]
        public void List_OfDerived_IsViewOfBase()
        {
            object derived = new List<VarDerived>();
            object bases = new List<VarBase>();

            Assert.IsTrue(derived is IReadOnlyList<VarBase>);
            Assert.IsTrue(derived is IEnumerable<VarBase>);
            Assert.IsFalse(derived is IReadOnlyList<VarOther>);
            Assert.IsFalse(bases is IReadOnlyList<VarDerived>);
            Assert.IsNull(bases as IReadOnlyList<VarDerived>);
            Assert.IsNotNull(derived as IReadOnlyList<VarBase>);
        }

        [TestMethod]
        public void List_Cast_ToWrongVariantInterface_Throws()
        {
            object bases = new List<VarBase>();
            object ints = new List<int>();

            Assert.ThrowsException(typeof(InvalidCastException), () => { _ = (IReadOnlyList<VarDerived>)bases; });
            Assert.ThrowsException(typeof(InvalidCastException), () => { _ = (IEnumerable<object>)ints; });
        }

        [TestMethod]
        public void List_ThroughCovariantView_Reads()
        {
            VarDerived first = new VarDerived();
            VarDerived second = new VarDerived();
            List<VarDerived> list = new List<VarDerived>();
            list.Add(first);
            list.Add(second);

            IReadOnlyList<VarBase> view = list;

            Assert.AreEqual(2, view.Count);
            Assert.IsTrue(object.ReferenceEquals(view[0], first));
            Assert.IsTrue(object.ReferenceEquals(view[1], second));
        }

        [TestMethod]
        public void List_ThroughCovariantEnumerable_Iterates()
        {
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            list.Add("c");

            int count = 0;
            string joined = "";

            foreach (object item in (IEnumerable<object>)list)
            {
                count++;
                joined += item;
            }

            Assert.AreEqual(3, count);
            Assert.AreEqual("abc", joined);
        }

        [TestMethod]
        public void List_PassedAsCovariantArgument()
        {
            List<VarDerived> list = new List<VarDerived>();
            list.Add(new VarDerived());
            list.Add(new VarDerived());

            Assert.AreEqual(2, VarHelper.Count(list));
            Assert.AreEqual(2, VarHelper.CountObjects(new List<string>(new string[] { "x", "y" })));
        }

        [TestMethod]
        public void List_Constructor_FromCovariantCollection()
        {
            List<string> strings = new List<string>();
            strings.Add("a");
            strings.Add("b");

            List<object> objects = new List<object>(strings);

            Assert.AreEqual(2, objects.Count);
            Assert.AreEqual("a", objects[0]);
            Assert.AreEqual("b", objects[1]);

            VarDerived d = new VarDerived();
            List<VarDerived> derived = new List<VarDerived>();
            derived.Add(d);

            List<VarBase> bases = new List<VarBase>(derived);

            Assert.AreEqual(1, bases.Count);
            Assert.IsTrue(object.ReferenceEquals(bases[0], d));
        }

        [TestMethod]
        public void List_Constructor_FromSameSizeIntegralArray()
        {
            List<uint> list = new List<uint>((IEnumerable<uint>)(object)new int[] { 1, 2, 3 });

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1u, list[0]);
            Assert.AreEqual(3u, list[2]);
        }

        [TestMethod]
        public void Dictionary_Collections_AreCovariantViews()
        {
            Dictionary<string, VarDerived> dictionary = new Dictionary<string, VarDerived>();
            dictionary.Add("a", new VarDerived());

            object keys = dictionary.Keys;
            object values = dictionary.Values;

            Assert.IsTrue(keys is IReadOnlyCollection<object>);
            Assert.IsTrue(keys is IEnumerable<object>);
            Assert.IsFalse(keys is ICollection<object>);
            Assert.IsTrue(values is IReadOnlyCollection<VarBase>);
            Assert.IsTrue(values is IEnumerable<VarBase>);
            Assert.IsFalse(values is ICollection<VarBase>);
            Assert.IsFalse(values is IEnumerable<VarOther>);
        }

        [TestMethod]
        public void Dictionary_IsNotCovariantInValueOrPair()
        {
            object dictionary = new Dictionary<string, VarDerived>();

            Assert.IsTrue(dictionary is IDictionary<string, VarDerived>);
            Assert.IsTrue(dictionary is IReadOnlyDictionary<string, VarDerived>);
            Assert.IsTrue(dictionary is IEnumerable<KeyValuePair<string, VarDerived>>);
            Assert.IsFalse(dictionary is IDictionary<string, VarBase>);
            Assert.IsFalse(dictionary is IReadOnlyDictionary<string, VarBase>);
            Assert.IsFalse(dictionary is IEnumerable<KeyValuePair<string, VarBase>>);
            Assert.IsFalse(dictionary is IEnumerable<object>);
        }

        [TestMethod]
        public void Dictionary_OfValueType_Values_AreNotCovariant()
        {
            object values = new Dictionary<string, int>().Values;

            Assert.IsTrue(values is IEnumerable<int>);
            Assert.IsFalse(values is IEnumerable<object>);
            Assert.IsFalse(values is IEnumerable<long>);
        }

        [TestMethod]
        public void Contravariance_ComparableOfBase_IsComparableOfDerived()
        {
            object comparer = new VarComparer();

            Assert.IsTrue(comparer is IComparable<VarBase>);
            Assert.IsTrue(comparer is IComparable<VarDerived>);
            Assert.IsFalse(comparer is IComparable<VarOther>);
            Assert.IsFalse(comparer is IComparable<object>);
        }

        [TestMethod]
        public void Array_OfDerived_IsViewOfBase()
        {
            object derived = new VarDerived[2];

            Assert.IsTrue(derived is IList<VarBase>);
            Assert.IsTrue(derived is IReadOnlyList<VarBase>);
            Assert.IsTrue(derived is IEnumerable<VarBase>);
            Assert.IsFalse(derived is IList<VarOther>);
            object bases = new VarBase[1];

            Assert.IsFalse(bases is IList<VarDerived>);
        }

        [TestMethod]
        public void Array_ThroughBaseView_ReadsAndStoresCompatible()
        {
            VarDerived first = new VarDerived();
            VarDerived[] array = new VarDerived[2];
            IList<VarBase> list = (IList<VarBase>)(object)array;

            list[0] = first;

            Assert.IsTrue(object.ReferenceEquals(array[0], first));
            Assert.IsTrue(object.ReferenceEquals(list[0], first));
            Assert.AreEqual(0, list.IndexOf(first));
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Array_ThroughBaseView_RejectsIncompatibleStore()
        {
            IList<VarBase> list = (IList<VarBase>)(object)new VarDerived[1];

            Assert.ThrowsException(typeof(Exception), () => { list[0] = new VarBase(); });
        }

        [TestMethod]
        public void Array_OfSameSizeIntegral_IsViewOfOtherSign()
        {
            object ints = new int[1];
            object bytes = new byte[1];
            object longs = new long[1];

            Assert.IsTrue(ints is IList<uint>);
            Assert.IsTrue(ints is IReadOnlyList<uint>);
            Assert.IsTrue(ints is IEnumerable<int>);
            Assert.IsTrue(bytes is IList<sbyte>);
            Assert.IsTrue(longs is IList<ulong>);
            Assert.IsFalse(ints is IList<long>);
            Assert.IsFalse(ints is IList<short>);
            Assert.IsFalse(bytes is IList<short>);
            Assert.IsFalse(ints is IList<VarEnumLong>);
        }

        [TestMethod]
        public void Array_OfSameSizeIntegral_ReadsAndWrites()
        {
            int[] array = new int[] { 1, -1, 3 };
            IList<uint> list = (IList<uint>)(object)array;

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1u, list[0]);
            Assert.AreEqual(3u, list[2]);

            list[1] = 5u;
            Assert.AreEqual(5, array[1]);
        }

        [TestMethod]
        public void Array_OfEnum_IsViewOfUnderlyingType()
        {
            object enums = new VarEnum[2];
            object ints = new int[2];

            Assert.IsTrue(enums is IList<int>);
            Assert.IsTrue(enums is IList<uint>);
            Assert.IsTrue(ints is IList<VarEnum>);
            Assert.IsFalse(enums is IList<long>);
            Assert.IsFalse(enums is IList<VarEnumLong>);
        }

        [TestMethod]
        public void Array_OfEnum_ReadsThroughUnderlyingView()
        {
            VarEnum[] enums = new VarEnum[] { VarEnum.A, VarEnum.B };
            IList<int> list = (IList<int>)(object)enums;

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(1, list.IndexOf(2));
        }

        [TestMethod]
        public void Array_Jagged_IsView()
        {
            object strings = new string[1][];
            object lists = new List<VarDerived>[1];

            Assert.IsTrue(strings is IList<object[]>);
            Assert.IsTrue(strings is IList<IList<object>>);
            Assert.IsTrue(strings is IList<IReadOnlyList<object>>);
            Assert.IsFalse(strings is IList<int[]>);
            Assert.IsTrue(lists is IList<IReadOnlyList<VarBase>>);
            Assert.IsFalse(lists is IList<IList<VarBase>>);
        }

        [TestMethod]
        public void Array_OfGenericInstances_IsCovariant()
        {
            object lists = new IReadOnlyList<VarDerived>[1];

            Assert.IsTrue(lists is IReadOnlyList<VarDerived>[]);
            Assert.IsTrue(lists is IReadOnlyList<VarBase>[]);
            Assert.IsTrue(lists is IEnumerable<VarBase>[]);
            Assert.IsTrue(lists is object[]);
            Assert.IsFalse(lists is IList<VarBase>[]);
            Assert.IsFalse(lists is IReadOnlyList<VarOther>[]);
            Assert.IsFalse(new IReadOnlyList<int>[1] is IReadOnlyList<long>[]);
        }

        [TestMethod]
        public void Array_Store_ThroughCovariantArray()
        {
            IEnumerable<VarBase>[] target = new List<VarDerived>[1];

            target[0] = new List<VarDerived>();

            Assert.IsNotNull(target[0]);
            Assert.ThrowsException(typeof(Exception), () => { target[0] = new List<VarBase>(); });
        }

        [TestMethod]
        public void Array_Copy_SameSizeIntegral()
        {
            int[] source = new int[] { 1, 2, 3 };
            uint[] target = new uint[3];

            Array.Copy(source, target, 3);

            Assert.AreEqual(1u, target[0]);
            Assert.AreEqual(3u, target[2]);
        }

        [TestMethod]
        public void Array_Copy_DerivedToBase()
        {
            VarDerived item = new VarDerived();
            VarDerived[] source = new VarDerived[] { item, null };
            VarBase[] target = new VarBase[2];

            Array.Copy(source, target, 2);

            Assert.IsTrue(object.ReferenceEquals(target[0], item));
            Assert.IsNull(target[1]);
        }

        [TestMethod]
        public void Type_NestedGenericArguments()
        {
            Type type = typeof(Dictionary<string, List<int[]>>);
            Type[] arguments = type.GetGenericArguments();

            Assert.AreEqual(2, arguments.Length);
            Assert.IsTrue(arguments[0] == typeof(string));
            Assert.IsTrue(arguments[1] == typeof(List<int[]>));
            Assert.IsTrue(arguments[1].GetGenericArguments()[0] == typeof(int[]));
            Assert.IsFalse(arguments[1] == typeof(List<int>));
            Assert.IsFalse(arguments[1] == typeof(List<uint[]>));
        }

        [TestMethod]
        public void Type_OfT_InsideGenericMethod()
        {
            Assert.IsTrue(VarHelper.Of<List<int>>() == typeof(List<int>));
            Assert.IsTrue(VarHelper.Of<List<VarDerived>>() == new List<VarDerived>().GetType());
            Assert.IsTrue(VarHelper.Of<IReadOnlyList<VarBase>>() == typeof(IReadOnlyList<VarBase>));
            Assert.IsFalse(VarHelper.Of<IReadOnlyList<VarBase>>() == typeof(IReadOnlyList<VarDerived>));
            Assert.IsTrue(VarHelper.Of<Dictionary<string, int>>() == typeof(Dictionary<string, int>));
            Assert.IsTrue(VarHelper.Of<VarDerived[]>() == typeof(VarDerived[]));
        }

        [TestMethod]
        public void Type_OfT_InsideGenericType()
        {
            Assert.IsTrue(new VarHolder<List<int>>().Type == typeof(List<int>));
            Assert.IsTrue(new VarHolder<VarDerived>().Type == typeof(VarDerived));
            Assert.IsFalse(new VarHolder<VarDerived>().Type == new VarHolder<VarBase>().Type);
            Assert.IsTrue(new VarHolder<VarDerived>().ListType == typeof(List<VarDerived>));
            Assert.IsTrue(new VarHolder<VarDerived>().ArrayType == typeof(VarDerived[]));
        }

        [TestMethod]
        public void Type_BaseType()
        {
            Assert.IsTrue(typeof(VarDerived).BaseType == typeof(VarBase));
            Assert.IsTrue(typeof(List<int>).BaseType == typeof(object));
            Assert.IsTrue(typeof(VarHolderDerived).BaseType == typeof(VarHolder<VarBase>));
            Assert.IsFalse(typeof(VarHolderDerived).BaseType == typeof(VarHolder<VarDerived>));
        }

        [TestMethod]
        public void Type_ArrayOfGenericInstance()
        {
            Type type = new List<string>[1].GetType();

            Assert.IsTrue(type.IsArray);
            Assert.IsTrue(type == typeof(List<string>[]));
            Assert.IsTrue(type.GetElementType() == typeof(List<string>));
            Assert.IsFalse(type == typeof(List<object>[]));
            Assert.IsFalse(type == typeof(List<string>[][]));
        }

        [TestMethod]
        public void Type_NestedCollectionIdentity()
        {
            Dictionary<string, VarDerived> dictionary = new Dictionary<string, VarDerived>();

            Assert.IsTrue(dictionary.Keys.GetType() == typeof(Dictionary<string, VarDerived>.KeyCollection));
            Assert.IsTrue(dictionary.Values.GetType() == typeof(Dictionary<string, VarDerived>.ValueCollection));
            Assert.IsFalse(dictionary.Values.GetType() == typeof(Dictionary<string, VarBase>.ValueCollection));
            Assert.IsFalse(dictionary.Keys.GetType() == dictionary.Values.GetType());
        }

        [TestMethod]
        public void Type_IsInstanceOfType_FollowsVariance()
        {
            Assert.IsTrue(typeof(IEnumerable<object>).IsInstanceOfType(new List<string>()));
            Assert.IsTrue(typeof(IReadOnlyList<VarBase>).IsInstanceOfType(new List<VarDerived>()));
            Assert.IsFalse(typeof(IList<object>).IsInstanceOfType(new List<string>()));
            Assert.IsFalse(typeof(IEnumerable<object>).IsInstanceOfType(new List<int>()));
        }

        [TestMethod]
        public void Type_GenericIdentity()
        {
            List<VarDerived> list = new List<VarDerived>();

            Assert.IsTrue(list.GetType() == typeof(List<VarDerived>));
            Assert.IsFalse(list.GetType() == typeof(List<VarBase>));
            Assert.IsTrue(list.GetType().IsGenericType);
            Assert.IsTrue(list.GetType().GetGenericTypeDefinition() == typeof(List<>));
            Assert.AreEqual(1, list.GetType().GetGenericArguments().Length);
            Assert.IsTrue(list.GetType().GetGenericArguments()[0] == typeof(VarDerived));
        }

        [TestMethod]
        public void Type_GenericIdentity_Dictionary()
        {
            Type type = typeof(Dictionary<string, VarDerived>);
            Type[] arguments = type.GetGenericArguments();

            Assert.IsTrue(type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
            Assert.AreEqual(2, arguments.Length);
            Assert.IsTrue(arguments[0] == typeof(string));
            Assert.IsTrue(arguments[1] == typeof(VarDerived));
            Assert.IsFalse(type == typeof(Dictionary<string, VarBase>));
        }

        [TestMethod]
        public void Type_InterfaceIdentity_IgnoresVariance()
        {
            Assert.IsFalse(typeof(IReadOnlyList<VarBase>) == typeof(IReadOnlyList<VarDerived>));
            Assert.IsTrue(typeof(IReadOnlyList<VarBase>) == typeof(IReadOnlyList<VarBase>));
            Assert.IsTrue(typeof(IReadOnlyList<>).IsGenericTypeDefinition);
        }
    }

    public class VarBase { }

    public class VarDerived : VarBase { }

    public class VarOther { }

    public enum VarEnum : int
    {
        A = 1,
        B = 2,
    }

    public enum VarEnumLong : long
    {
        A = 1,
    }

    public class VarComparer : IComparable<VarBase>
    {
        public int CompareTo(VarBase other) => 7;
    }

    public static class VarHelper
    {
        public static Type Of<T>() => typeof(T);

        public static int Count(IEnumerable<VarBase> items)
        {
            int count = 0;

            foreach (VarBase item in items)
            {
                count++;
            }

            return count;
        }

        public static int CountObjects(IEnumerable<object> items)
        {
            int count = 0;

            foreach (object item in items)
            {
                count++;
            }

            return count;
        }
    }

    public class VarHolder<T>
    {
        public Type Type => typeof(T);

        public Type ListType => typeof(List<T>);

        public Type ArrayType => typeof(T[]);
    }

    public class VarHolderDerived : VarHolder<VarBase> { }
}
