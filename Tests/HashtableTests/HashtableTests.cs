//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Collections;

namespace NFUnitTests
{
    [TestClass]
    public class HashtableTests
    {
        private const int c_MinimumEntries = 10;
        private const int c_BareMinimum = 1;

        [Setup]
        public void SetupTests()
        {
            // need this empty method to get everything setup in the class
            // otherwise the 1st test will incur in that time penalty thus not proving an accurate execution time
        }

        [TestMethod]
        public void Ctor_Empty()
        {
            var hash = new ComparableHashtable();
            VerifyHashtable(hash, null);
        }

        [TestMethod]
        public void Ctor_IDictionary()
        {
            // No exception
            var hash1 = new ComparableHashtable(new Hashtable());
            Assert.Equal(0, hash1.Count);

            hash1 = new ComparableHashtable(new Hashtable());
            Assert.Equal(0, hash1.Count);

            Hashtable hash2 = CreateIntHashtable(100);
            hash1 = new ComparableHashtable(hash2);
            VerifyHashtable(hash1, hash2);
        }

        [TestMethod]
        public void Ctor_Int()
        {
            var hash = new ComparableHashtable(0);
            VerifyHashtable(hash, null);

            hash = new ComparableHashtable(10);
            VerifyHashtable(hash, null);

            hash = new ComparableHashtable(100);
            VerifyHashtable(hash, null);
        }

        [TestMethod]
        public void Ctor_Int_Invalid()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException),
                () =>
                {
                    new Hashtable(-1);
                });

            Assert.Throws(typeof(ArgumentException),
                () =>
                {
                    new Hashtable(int.MaxValue);
                });
        }

        [TestMethod]
        public void Add_ReferenceType()
        {
            var hash1 = new Hashtable();

            Hashtable[] hashtableTypes =
            {
                (Hashtable)hash1.Clone(),
            };

            foreach (Hashtable hashtableType in hashtableTypes)
            {
                // Value is a reference
                var foo = new Foo();
                hashtableType.Add("Key", foo);

                Assert.Equal("Hello World", ((Foo)hashtableType["Key"]).StringValue);

                // Changing original object should change the object stored in the Hashtable
                foo.StringValue = "Goodbye";
                Assert.Equal("Goodbye", ((Foo)hashtableType["Key"]).StringValue);
            }
        }

        [TestMethod]
        public void Add_ClearRepeatedly()
        {
            const int Iterations = 2;
            const int Count = 2;

            var hash = new Hashtable();

            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    string key = "Key: i=" + i + ", j=" + j;
                    string value = "Value: i=" + i + ", j=" + j;
                    hash.Add(key, value);
                }

                Assert.Equal(Count, hash.Count);
                hash.Clear();
            }
        }

        [TestMethod]
        public void AddRemove_LargeAmountNumbers()
        {
            // Generate a random 1000 array of ints as test data
            var inputData = new int[1_000];
            var random = new Random();
            inputData[0] = 0;
            for (int i = 1; i < inputData.Length; i++)
            {
                inputData[i] = inputData[i - 1] + 1 + random.Next(100);
            }

            var hash = new Hashtable();

            int count = 0;
            foreach (long number in inputData)
            {
                hash.Add(number, count++);
            }

            count = 0;
            foreach (long number in inputData)
            {
                Assert.True(hash[number].Equals(count));
                Assert.True(hash.Contains(number));

                count++;
            }

            foreach (long number in inputData)
            {
                hash.Remove(number);
            }

            Assert.Equal(0, hash.Count);
        }

        [TestMethod]
        public void Clone()
        {
            // empty hashtable
            Hashtable hash1 = CreateStringHashtable(0);

            Hashtable[] hashtableTypes =
            {
                (Hashtable)hash1.Clone()
            };

            foreach (Hashtable hashtableType in hashtableTypes)
            {
                Hashtable clone = (Hashtable)hashtableType.Clone();

                Assert.Equal(hashtableType.Count, clone.Count);
                Assert.Equal(hashtableType.IsSynchronized, clone.IsSynchronized);
                Assert.Equal(hashtableType.IsFixedSize, clone.IsFixedSize);
                Assert.Equal(hashtableType.IsReadOnly, clone.IsReadOnly);

                for (int i = 0; i < clone.Count; i++)
                {
                    string key = "Key_" + i;
                    string value = "Value_" + i;

                    Assert.True(clone.Contains(key));
                    // TODO need ContainsValue
                    //Assert.NotNull(clone.ContainsValue(value));
                    Assert.True(value.Equals(clone[key]));
                }
            }

            // hashtable with 100 elements
            hash1 = CreateStringHashtable(100);

            hashtableTypes = new Hashtable[]
            {
                (Hashtable)hash1.Clone()
            };

            foreach (Hashtable hashtableType in hashtableTypes)
            {
                Hashtable clone = (Hashtable)hashtableType.Clone();

                Assert.Equal(hashtableType.Count, clone.Count);
                Assert.Equal(hashtableType.IsSynchronized, clone.IsSynchronized);
                Assert.Equal(hashtableType.IsFixedSize, clone.IsFixedSize);
                Assert.Equal(hashtableType.IsReadOnly, clone.IsReadOnly);

                for (int i = 0; i < clone.Count; i++)
                {
                    string key = "Key_" + i;
                    string value = "Value_" + i;

                    Assert.True(clone.Contains(key));
                    // TODO need ContainsValue
                    //Assert.NotNull(clone.ContainsValue(value));
                    Assert.True(value.Equals(clone[key]));
                }
            }
        }

        [TestMethod]
        public void Clone_IsShallowCopy()
        {
            var hash = new Hashtable();
            for (int i = 0; i < 10; i++)
            {
                hash.Add(i, new Foo());
            }

            Hashtable clone = (Hashtable)hash.Clone();
            for (int i = 0; i < clone.Count; i++)
            {
                Assert.Equal("Hello World", ((Foo)clone[i]).StringValue);
                Assert.Same(hash[i], clone[i]);
            }

            // Change object in original hashtable
            ((Foo)hash[1]).StringValue = "Goodbye";
            Assert.Equal("Goodbye", ((Foo)clone[1]).StringValue);

            // Removing an object from the original hashtable doesn't change the clone
            hash.Remove(0);
            Assert.True(clone.Contains(0));
        }

        [TestMethod]
        public void Clone_HashtableCastedToInterfaces()
        {
            // Try to cast the returned object from Clone() to different types
            Hashtable hash = CreateIntHashtable(100);

            ICollection collection = (ICollection)hash.Clone();
            Assert.Equal(hash.Count, collection.Count);

            IDictionary dictionary = (IDictionary)hash.Clone();
            Assert.Equal(hash.Count, dictionary.Count);
        }

        [TestMethod]
        public void ContainsKey()
        {
            Hashtable hash1 = CreateStringHashtable(100);

            Hashtable[] hashtableTypes =
            {
                (Hashtable)hash1.Clone()
            };

            foreach (Hashtable hashtableType in hashtableTypes)
            {
                for (int i = 0; i < hashtableType.Count; i++)
                {
                    string key = "Key_" + i;

                    Assert.True(hashtableType.Contains(key), $"Expecting to find {key} key");
                }

                Assert.False(hashtableType.Contains("Non Existent Key"));

                Assert.False(hashtableType.Contains(101));
                Assert.False(hashtableType.Contains("Non Existent Key"));

                string removedKey = "Key_1";
                hashtableType.Remove(removedKey);

                Assert.False(hashtableType.Contains(removedKey), $"Hashtacble contains {removedKey} when it shouldn't.");
            }
        }

        [TestMethod]
        public void ContainsKey_NullKey_ThrowsArgumentNullException()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                () =>
                {
                    var hash1 = new Hashtable();

                    Hashtable[] hashtableTypes =
                    {
                        (Hashtable)hash1.Clone()
                    };

                    foreach (Hashtable hashtableType in hashtableTypes)
                    {
                        hashtableType.Contains(null);
                    }
                });
        }

        [TestMethod]
        public void Keys_ModifyingHashtable_ModifiesCollection()
        {
            Hashtable hash = CreateStringHashtable(100);

            ICollection keys = hash.Keys;

            // Removing a key from the hashtable should update the Keys ICollection.
            // This means that the Keys ICollection no longer contains the key.
            hash.Remove("Key_0");

            IEnumerator enumerator = keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.False(enumerator.Current.Equals("Key_0"));
            }
        }

        [TestMethod]
        public void Values_ModifyingHashtable_ModifiesCollection()
        {
            Hashtable hash = CreateStringHashtable(100);

            ICollection values = hash.Values;

            // Removing a value from the hashtable should update the Values ICollection.
            // This means that the Values ICollection no longer contains the value.
            hash.Remove("Key_0");

            IEnumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.False(enumerator.Current.Equals("Value_0"));
            }
        }

        [TestMethod]
        public void Values_ModifyingHashtable_WhileEnumerating()
        {
            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    Hashtable hash = CreateStringHashtable(10);

                    foreach (var key in hash.Keys)
                    {
                        hash[key] = Guid.NewGuid().ToString();
                    }
                });
        }

        [TestMethod]
        public void Hashtable_Add_Contains_Remove_Test1()
        {
            // Enter 5 values with 5 unique keys
            // 1) check that the keys are in the table
            // 2) check that all keys are present
            // 3) check that the items are what they are expected to be through all possible interfaces
            // 4) check that we can successfully remove the items 
            // 5) check that a removed item is no longer in the table, and so its key it is no longer in the table as well

            string key1 = "key1";
            string key2 = "key2";
            string key3 = "key3";
            string key4 = "key4";
            string key5 = "key5";

            MyClassTypeEntry entry1 = new MyClassTypeEntry("1 (one)", 1, Guid.NewGuid());
            MyClassTypeEntry entry2 = new MyClassTypeEntry("2 (two)", 2, Guid.NewGuid());
            MyClassTypeEntry entry3 = new MyClassTypeEntry("3 (three)", 3, Guid.NewGuid());
            MyClassTypeEntry entry4 = new MyClassTypeEntry("4 (four)", 4, Guid.NewGuid());
            MyClassTypeEntry entry5 = new MyClassTypeEntry("5 (five)", 5, Guid.NewGuid());

            string[] keys = new string[] { key1, key2, key3, key4, key5 };
            MyClassTypeEntry[] entries = new MyClassTypeEntry[] { entry1, entry2, entry3, entry4, entry5 };

            Hashtable t = new Hashtable();

            // 1) add 5 items with 5 unique keys
            t.Add(key1, entry1);
            t.Add(key2, entry2);
            t.Add(key3, entry3);
            t.Add(key4, entry4);
            t.Add(key5, entry5);

            // 2) check all added keys are present
            Assert.True(t.Contains(key1), $"Key '{key1}' not found in Hashtable.");
            Assert.True(t.Contains(key2), $"Key '{key2}' not found in Hashtable.");
            Assert.True(t.Contains(key3), $"Key '{key3}' not found in Hashtable.");
            Assert.True(t.Contains(key4), $"Key '{key4}' not found in Hashtable.");
            Assert.True(t.Contains(key5), $"Key '{key5}' not found in Hashtable.");

            // 3) check that the items are what they are expected to be
            // check the items reference and value first...
            int index = 0;

            foreach (string k in keys)
            {
                // test indexer
                MyClassTypeEntry entry = (MyClassTypeEntry)t[k];

                // check that the reference is the same 
                Assert.True(Object.ReferenceEquals(entry, entries[index]));

                // check that the values are the same
                Assert.True(entry.Equals(entries[index]), "Values don't match");

                index++;
            }

            // ... then check the keys
            foreach (string k in keys)
            {
                bool found = false;
                ICollection keysCollection = t.Keys;

                foreach (string key in keysCollection)
                {
                    if (k == key)
                    {
                        found = true;
                        break;
                    }
                }

                Assert.True(found, $"Couldn't find '{k}' key.");
            }

            // 4) checked that we can remove the items
            // ... then check the keys                
            foreach (string k in keys)
            {
                t.Remove(k);
            }

            // 4) checked that we can remove the items
            // ... then check the keys                
            foreach (string k in keys)
            {
                t.Remove(k);
            }

            // 5) check that a removed item is no longer in the table, and so its key it is no longer in the table as well           
            // check the items reference and value first...
            // test nothing is left in the Hashtable 
            Assert.True(t.Count == 0, "Hashtable shouldn't have elements, but it has.");

            foreach (string k in keys)
            {
                // test Contains
                Assert.False(t.Contains(k), $"Hashtable contains '{k}' key, but it shouldn't.");

                // test indexer
                MyClassTypeEntry entry = (MyClassTypeEntry)t[k];

                Assert.Null(entry, "'entry' should be null, but is not.");
            }
        }

        [TestMethod]
        public void Hashtable_Add_Clear()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            Assert.True(t.Count == vals.Length, "Hashtable and collection should have the same number of elements, but they don't.");

            t.Clear();

            Assert.True(t.Count == 0);

            ICollection keys = t.Keys;
            ICollection values = t.Values;

            Assert.True(keys.Count == 0, "Hashtable should have no keys, but it has.");

            Assert.True(values.Count == 0, "Hashtable should have no values, but it has.");
        }

        [TestMethod]
        public void Hashtable_CheckKeys()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            // check that the hastable contains the keys
            foreach (MyClassTypeEntry k in vals)
            {
                Assert.True(t.Contains(k.StringValue), $"Hashtable doesn't contain key '{k.StringValue}' but it should.");
            }

            ICollection keys = t.Keys;

            foreach (MyClassTypeEntry m in vals)
            {
                // check that the key collection contains the key
                bool found = false;

                foreach (string s in keys)
                {
                    if (m.StringValue.Equals(s))
                    {
                        found = true; break;
                    }
                }

                Assert.True(found, $"Collection doesn't contain key '{m.StringValue}' but it should.");
            }
        }

        [TestMethod]
        public void Hashtable_CheckValues()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            // check that the hastable contains the keys
            foreach (MyClassTypeEntry k in vals)
            {
                Assert.True(t.Contains(k.StringValue), $"Hashtable doesn't contain element '{k.StringValue}' but it should.");
            }

            ICollection values = t.Values;

            foreach (MyClassTypeEntry m in vals)
            {
                // check that the key collection contains the key
                bool verified = false;

                foreach (MyClassTypeEntry mm in values)
                {
                    if (m.Equals(mm))
                    {
                        verified = true;
                        break;
                    }
                }

                Assert.True(verified, $"Collection doesn't contain element '{m.StringValue}' but it should.");
            }
        }

        [TestMethod]
        public void Hashtable_Count()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            int count = t.Count;

            Assert.True(vals.Length == count);

            t.Add("a new key without a guid, can't exist", new MyClassTypeEntry());
            t.Add("a new key without a guid, can't exist again", new MyClassTypeEntry());
            t.Add("a new key without a guid, can't exist another time", new MyClassTypeEntry());

            Assert.True((count + 3) == t.Count, "Number of elements doesn't match.");
        }

        [TestMethod]
        public void Hashtable_Duplicate_01()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, 10);

            // find a key and insert a duplicate: must fail
            MyClassTypeEntry entry = vals[vals.Length / 2];
            string key = MyClassTypeEntry.GetKey(entry.IntegerValue, entry.GuidValue);

            Assert.Throws(typeof(ArgumentException), () =>
            {
                t.Add(key, new MyClassTypeEntry());

            }, "No exception was thrown when adding a duplicate element.");

            // remove the item 
            t.Remove(key);

            t.Add(key, new MyClassTypeEntry());
        }

        [TestMethod]
        public void Hashtable_Duplicate_02()
        {
            // Try to cast the returned object from Clone() to different types
            Hashtable ht = CreateIntHashtable(100);

            int duplicate = ht.Count / 4;

            Assert.Throws(typeof(ArgumentException), () =>
            {
                ht.Add(duplicate, duplicate);

            }, "No exception was thrown when adding a duplicate element.");

            // remove the item 
            ht.Remove(duplicate);

            ht.Add(duplicate, duplicate);

            duplicate = ht.Count / 2;

            Assert.Throws(typeof(ArgumentException), () =>
            {
                ht.Add(duplicate, duplicate);

            }, "No exception was thrown when adding a duplicate element.");

            // remove the item 
            ht.Remove(duplicate);

            ht.Add(duplicate, duplicate);

        }

        [TestMethod]
        public void Hashtable_CopyTo()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            string[] keys = new string[t.Count];
            t.Keys.CopyTo(keys, 0);

            int countOfKeysFound = 0;

            // check that all keys have been copied
            foreach (var masterKey in t.Keys)
            {
                bool masterKeyFound = false;

                foreach (var key in keys)
                {
                    if (key == (string)masterKey)
                    {
                        masterKeyFound = true;

                        countOfKeysFound++;

                        break;
                    }
                }

                Assert.True(masterKeyFound, $"Couldn't find key {masterKey} in the Array copy.");
            }

            // all keys should have been found 
            Assert.True(countOfKeysFound == t.Count, $"Keys count don't match. Hashtable has {t.Count} and was expecting {countOfKeysFound}");
        }

        #region helper classes and methods

        /// <summary>
        /// Creates a MyClassEntry type whose string member with the prefix is the key item in the hashtable
        /// </summary>
        private MyClassTypeEntry[] InsertRandomValues(Hashtable t, int max)
        {
            int count = (new Random().Next() % max) + c_BareMinimum;

            MyClassTypeEntry[] vals = new MyClassTypeEntry[count];

            for (int i = 0; i < count; ++i)
            {
                Guid g = Guid.NewGuid();
                string key = MyClassTypeEntry.GetKey(i, g);
                vals[i] = new MyClassTypeEntry(key, i, g);
                t.Add(key, vals[i]);
            }

            return vals;
        }

        private static void VerifyHashtable(ComparableHashtable hash1, Hashtable hash2)
        {
            if (hash2 == null)
            {
                Assert.Equal(0, hash1.Count, "Element count is different.");
            }
            else
            {
                // Make sure that construtor imports all keys and values
                Assert.Equal(hash2.Count, hash1.Count);

                for (int i = 0; i < 100; i++)
                {
                    Assert.True(hash1.Contains(i));
                    Assert.NotNull(hash1[i]);
                }

                // Make sure the new and old hashtables are not linked
                hash2.Clear();

                for (int i = 0; i < 100; i++)
                {
                    Assert.True(hash1.Contains(i));
                    Assert.NotNull(hash1[i]);
                }
            }

            Assert.False(hash1.IsFixedSize);
            Assert.False(hash1.IsReadOnly);
            Assert.False(hash1.IsSynchronized);

            // Make sure we can add to the hashtable
            int count = hash1.Count;
            for (int i = count; i < count + 100; i++)
            {
                hash1.Add(i, i);

                Assert.True(hash1.Contains(i));
                Assert.NotNull(hash1[i]);
            }
        }

        private class ComparableHashtable : Hashtable
        {
            public ComparableHashtable() : base() { }

            public ComparableHashtable(Hashtable d) : base()
            {
                foreach (var itemKey in d.Keys)
                {
                    Add(itemKey, d[itemKey]);
                }
            }

            public ComparableHashtable(int capacity) : base(capacity) { }
        }

        public static Hashtable CreateIntHashtable(int count, int start = 0)
        {
            var hashtable = new Hashtable();

            for (int i = start; i < start + count; i++)
            {
                hashtable.Add(i, i);
            }

            return hashtable;
        }

        public static Hashtable CreateStringHashtable(int count, int start = 0)
        {
            var hashtable = new Hashtable();

            for (int i = start; i < start + count; i++)
            {
                string key = "Key_" + i;
                string value = "Value_" + i;

                hashtable.Add(key, value);
            }

            return hashtable;
        }

        private class Foo
        {
            public string StringValue { get; set; } = "Hello World";

            public override bool Equals(object obj)
            {
                Foo foo = obj as Foo;
                return foo != null && StringValue == foo.StringValue;
            }

            public override int GetHashCode() => StringValue.GetHashCode();
        }

        internal class MyClassTypeEntry
        {
            public MyClassTypeEntry()
            {
                m_structValue = Guid.NewGuid();
                m_stringValue = "string" + m_structValue.ToString();
                m_integralValue = 42;
            }

            public MyClassTypeEntry(string s, int i, Guid g)
            {
                m_stringValue = s;
                m_integralValue = i;
                m_structValue = g;
            }

            // override Object.GetHashCode
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            // override Object.Equals
            public override bool Equals(object obj)
            {
                try
                {
                    MyClassTypeEntry a = (MyClassTypeEntry)obj;

                    if (m_stringValue != a.StringValue)
                    {
                        return false;
                    }
                    if (m_integralValue != a.IntegerValue)
                    {
                        return false;
                    }
                    if (!m_structValue.Equals(a.GuidValue))
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            public string StringValue
            {
                get
                {
                    return m_stringValue;
                }
            }
            public int IntegerValue
            {
                get
                {
                    return m_integralValue;
                }
            }
            public Guid GuidValue
            {
                get
                {
                    return m_structValue;
                }
            }
            //--//
            public static string GetKey(int i, Guid g)
            {
                return "key_" + i.ToString() + "__" + g.ToString();
            }
            //--//
            private readonly string m_stringValue;
            private readonly int m_integralValue;
            private readonly Guid m_structValue;

        }

        #endregion
    }
}
