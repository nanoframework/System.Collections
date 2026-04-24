// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;
using System;
using System.Collections.Generic;

namespace GenericCollections
{
    [TestClass]
    public class DictionaryTests
    {
        // -----------------------------------------------------------------------
        // Constructors
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Constructor_Default()
        {
            var dict = new Dictionary<int, string>();
            Assert.IsNotNull(dict);
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void Dictionary_Constructor_Capacity()
        {
            var dict = new Dictionary<string, int>(20);
            Assert.IsNotNull(dict);
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void Dictionary_Constructor_FromDictionary()
        {
            var source = new Dictionary<int, string>();
            source.Add(1, "one");
            source.Add(2, "two");
            source.Add(3, "three");

            var copy = new Dictionary<int, string>(source);
            Assert.AreEqual(3, copy.Count);
            Assert.AreEqual("one", copy[1]);
            Assert.AreEqual("two", copy[2]);
            Assert.AreEqual("three", copy[3]);
        }

        [TestMethod]
        public void Dictionary_Constructor_NullThrows()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () =>
            {
                var _ = new Dictionary<int, string>(null);
            });
        }

        // -----------------------------------------------------------------------
        // Add / Count
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Add_And_Count()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            Assert.AreEqual(3, dict.Count);
        }

        [TestMethod]
        public void Dictionary_Add_DuplicateKey_ThrowsArgumentException()
        {
            var dict = new Dictionary<int, int>();
            dict.Add(1, 10);

            Assert.ThrowsException(typeof(ArgumentException), () =>
            {
                dict.Add(1, 20);
            });
        }

        [TestMethod]
        public void Dictionary_Add_NullKey_ThrowsArgumentNullException()
        {
            var dict = new Dictionary<string, int>();

            Assert.ThrowsException(typeof(ArgumentNullException), () =>
            {
                dict.Add(null, 42);
            });
        }

        // -----------------------------------------------------------------------
        // Indexer
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Indexer_Get()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(10, "ten");
            dict.Add(20, "twenty");

            Assert.AreEqual("ten", dict[10]);
            Assert.AreEqual("twenty", dict[20]);
        }

        [TestMethod]
        public void Dictionary_Indexer_Set_NewKey()
        {
            var dict = new Dictionary<string, int>();
            dict["alpha"] = 1;
            dict["beta"] = 2;

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual(1, dict["alpha"]);
            Assert.AreEqual(2, dict["beta"]);
        }

        [TestMethod]
        public void Dictionary_Indexer_Set_ExistingKey_Updates()
        {
            var dict = new Dictionary<string, int>();
            dict["key"] = 100;
            dict["key"] = 200;

            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(200, dict["key"]);
        }

        [TestMethod]
        public void Dictionary_Indexer_KeyNotFound_ThrowsException()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");

            Assert.ThrowsException(typeof(InvalidOperationException), () =>
            {
                var _ = dict[99];
            });
        }

        // -----------------------------------------------------------------------
        // Remove
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Remove_ExistingKey()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            bool removed = dict.Remove(1);

            Assert.IsTrue(removed);
            Assert.AreEqual(1, dict.Count);
            Assert.IsFalse(dict.ContainsKey(1));
        }

        [TestMethod]
        public void Dictionary_Remove_NonExistingKey()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");

            bool removed = dict.Remove(99);

            Assert.IsFalse(removed);
            Assert.AreEqual(1, dict.Count);
        }

        [TestMethod]
        public void Dictionary_Remove_ThenAddSameKey()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("x", 10);
            dict.Remove("x");
            dict.Add("x", 20);

            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(20, dict["x"]);
        }

        // -----------------------------------------------------------------------
        // ContainsKey / ContainsValue
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_ContainsKey()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            Assert.IsTrue(dict.ContainsKey(1));
            Assert.IsTrue(dict.ContainsKey(2));
            Assert.IsFalse(dict.ContainsKey(3));
        }

        [TestMethod]
        public void Dictionary_ContainsValue()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            Assert.IsTrue(dict.ContainsValue("one"));
            Assert.IsTrue(dict.ContainsValue("two"));
            Assert.IsFalse(dict.ContainsValue("three"));
        }

        [TestMethod]
        public void Dictionary_ContainsValue_Null()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, null);

            Assert.IsTrue(dict.ContainsValue(null));
            Assert.IsFalse(dict.ContainsValue("something"));
        }

        // -----------------------------------------------------------------------
        // TryGetValue
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_TryGetValue_Found()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("hello", 42);

            bool found = dict.TryGetValue("hello", out int value);

            Assert.IsTrue(found);
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void Dictionary_TryGetValue_NotFound()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("hello", 42);

            bool found = dict.TryGetValue("world", out int value);

            Assert.IsFalse(found);
            Assert.AreEqual(default(int), value);
        }

        // -----------------------------------------------------------------------
        // Clear
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Clear()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            dict.Clear();

            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.ContainsKey(1));
            Assert.IsFalse(dict.ContainsKey(2));

            // Can add again after clear
            dict.Add(1, "one-again");
            Assert.AreEqual(1, dict.Count);
        }

        // -----------------------------------------------------------------------
        // Keys / Values collections
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Keys()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            var keys = dict.Keys;
            Assert.AreEqual(3, keys.Count);

            int[] keyArray = new int[3];
            keys.CopyTo(keyArray, 0);

            // Verify all keys are present (order is not guaranteed)
            bool found1 = false, found2 = false, found3 = false;
            foreach (int k in keyArray)
            {
                if (k == 1) found1 = true;
                if (k == 2) found2 = true;
                if (k == 3) found3 = true;
            }

            Assert.IsTrue(found1);
            Assert.IsTrue(found2);
            Assert.IsTrue(found3);
        }

        [TestMethod]
        public void Dictionary_Values()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            var values = dict.Values;
            Assert.AreEqual(3, values.Count);

            string[] valArray = new string[3];
            values.CopyTo(valArray, 0);

            // Verify all values are present (order is not guaranteed)
            bool foundOne = false, foundTwo = false, foundThree = false;
            foreach (string v in valArray)
            {
                if (v == "one") foundOne = true;
                if (v == "two") foundTwo = true;
                if (v == "three") foundThree = true;
            }

            Assert.IsTrue(foundOne);
            Assert.IsTrue(foundTwo);
            Assert.IsTrue(foundThree);
        }

        [TestMethod]
        public void Dictionary_Keys_Enumerator()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(10, "ten");
            dict.Add(20, "twenty");

            int sum = 0;
            foreach (int key in dict.Keys)
            {
                sum += key;
            }

            Assert.AreEqual(30, sum);
        }

        [TestMethod]
        public void Dictionary_Values_Enumerator()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "a");
            dict.Add(2, "b");
            dict.Add(3, "c");

            string concat = "";
            foreach (string val in dict.Values)
            {
                concat += val;
            }

            // Order not guaranteed — just check length/content
            Assert.AreEqual(3, concat.Length);
            Assert.IsTrue(concat.IndexOf("a") >= 0);
            Assert.IsTrue(concat.IndexOf("b") >= 0);
            Assert.IsTrue(concat.IndexOf("c") >= 0);
        }

        // -----------------------------------------------------------------------
        // Enumerator / foreach
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_Enumerator_Foreach()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            int count = 0;
            int keySum = 0;
            foreach (KeyValuePair<int, string> kvp in dict)
            {
                count++;
                keySum += kvp.Key;
            }

            Assert.AreEqual(3, count);
            Assert.AreEqual(6, keySum);
        }

        [TestMethod]
        public void Dictionary_Enumerator_ModifyThrows()
        {
            var dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);

            Assert.ThrowsException(typeof(InvalidOperationException), () =>
            {
                foreach (KeyValuePair<int, int> kvp in dict)
                {
                    dict.Add(99, 99);
                }
            });
        }

        // -----------------------------------------------------------------------
        // CopyTo
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_CopyTo()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            var dest = new KeyValuePair<int, string>[3];
            dict.CopyTo(dest, 1);

            // dest[0] should be default, dest[1] and dest[2] should have entries
            int filledCount = 0;
            for (int i = 1; i <= 2; i++)
            {
                if (dest[i].Key != 0 || dest[i].Value != null)
                {
                    filledCount++;
                }
            }

            Assert.AreEqual(2, filledCount);
        }

        [TestMethod]
        public void Dictionary_CopyTo_NullArray_Throws()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");

            Assert.ThrowsException(typeof(ArgumentNullException), () =>
            {
                dict.CopyTo(null, 0);
            });
        }

        [TestMethod]
        public void Dictionary_CopyTo_TooSmall_Throws()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            Assert.ThrowsException(typeof(ArgumentException), () =>
            {
                dict.CopyTo(new KeyValuePair<int, string>[2], 0);
            });
        }

        // -----------------------------------------------------------------------
        // ICollection<KVP> explicit interface methods
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_ICollectionKVP_Contains()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            ICollection<KeyValuePair<int, string>> col = dict;

            Assert.IsTrue(col.Contains(new KeyValuePair<int, string>(1, "one")));
            Assert.IsFalse(col.Contains(new KeyValuePair<int, string>(1, "wrong")));
            Assert.IsFalse(col.Contains(new KeyValuePair<int, string>(99, "one")));
        }

        [TestMethod]
        public void Dictionary_ICollectionKVP_Remove()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            ICollection<KeyValuePair<int, string>> col = dict;

            // Removing with wrong value should not remove
            bool removed = col.Remove(new KeyValuePair<int, string>(1, "wrong"));
            Assert.IsFalse(removed);
            Assert.AreEqual(2, dict.Count);

            // Removing with correct key+value should remove
            removed = col.Remove(new KeyValuePair<int, string>(1, "one"));
            Assert.IsTrue(removed);
            Assert.AreEqual(1, dict.Count);
        }

        // -----------------------------------------------------------------------
        // Large dictionary / resize
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dictionary_LargeCapacity_Growth()
        {
            var dict = new Dictionary<int, int>();

            for (int i = 0; i < 100; i++)
            {
                dict.Add(i, i * i);
            }

            Assert.AreEqual(100, dict.Count);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i * i, dict[i]);
            }
        }
    }
}
