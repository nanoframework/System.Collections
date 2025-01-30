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
            Assert.AreEqual(0, hash1.Count);

            hash1 = new ComparableHashtable(new Hashtable());
            Assert.AreEqual(0, hash1.Count);

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
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () =>
                {
                    new Hashtable(-1);
                });

            Assert.ThrowsException(typeof(ArgumentException),
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

                Assert.AreEqual("Hello World", ((Foo)hashtableType["Key"]).StringValue);

                // Changing original object should change the object stored in the Hashtable
                foo.StringValue = "Goodbye";
                Assert.AreEqual("Goodbye", ((Foo)hashtableType["Key"]).StringValue);
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

                Assert.AreEqual(Count, hash.Count);
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
                Assert.IsTrue(hash[number].Equals(count));
                Assert.IsTrue(hash.Contains(number));

                count++;
            }

            foreach (long number in inputData)
            {
                hash.Remove(number);
            }

            Assert.AreEqual(0, hash.Count);
        }


        [TestMethod]
        public void Add_ModifyKeyProperty_AddAgain()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey initialKey = new AnotherKey();

            // Act
            // Add component to collection
            hashtable.Add(initialKey, "some value");

            var initialKeyHashCode = initialKey.GetHashCode();
            OutputHelper.WriteLine($"Initial key: {initialKeyHashCode}");

            Assert.IsTrue(
                hashtable.Contains(initialKey),
                "Hashtable should contain the initial key.");

            // Modify key property
            initialKey.Value = 1.23456789;

            Assert.AreEqual(
                initialKeyHashCode,
                initialKey.GetHashCode(),
                "Hash code should not change after modifying the key.");

            // try to add the key again

            Assert.ThrowsException(
                typeof(ArgumentException),
                () =>
                {
                    hashtable.Add(initialKey, "some value");
                },
                "Adding the same key should throw an exception.");
        }

        [TestMethod]
        public void Add_SameHashCodeDifferentKeys()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key1" };
            SomeKey key2 = new SomeKey { KeyProperty = "key2" };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Add_SameKeyDifferentHashCodes()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key" };
            SomeKey key2 = new SomeKey { KeyProperty = "key" };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Remove(key1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Add_ModifyKeyHashCode()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key = new SomeKey { KeyProperty = "key" };
            string value = "value";

            // Act
            hashtable.Add(key, value);

            // Modify the key property which affects the hash code
            key.KeyProperty = "modifiedKey";

            // Assert
            Assert.IsFalse(hashtable.Contains(key), "Hashtable should not contain the modified key.");

            // Revert the key property to original
            key.KeyProperty = "key";

            Assert.IsTrue(hashtable.Contains(key), "Hashtable should contain the original key.");
            Assert.AreEqual(value, hashtable[key], "The value associated with the original key should be correct.");
        }

        [TestMethod]
        public void Add_AnotherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey key1 = new AnotherKey { id = 1, Value = 1.1 };
            AnotherKey key2 = new AnotherKey { id = 2, Value = 2.2 };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Add_Foo_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo key1 = new Foo { StringValue = "foo1" };
            Foo key2 = new Foo { StringValue = "foo2" };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Add_MyClassTypeEntry_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry key1 = new MyClassTypeEntry("string1", 1, Guid.NewGuid());
            MyClassTypeEntry key2 = new MyClassTypeEntry("string2", 2, Guid.NewGuid());
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
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

                Assert.AreEqual(hashtableType.Count, clone.Count);
                Assert.AreEqual(hashtableType.IsSynchronized, clone.IsSynchronized);
                Assert.AreEqual(hashtableType.IsFixedSize, clone.IsFixedSize);
                Assert.AreEqual(hashtableType.IsReadOnly, clone.IsReadOnly);

                for (int i = 0; i < clone.Count; i++)
                {
                    string key = "Key_" + i;
                    string value = "Value_" + i;

                    Assert.IsTrue(clone.Contains(key));
                    // TODO need ContainsValue
                    //Assert.IsNotNull(clone.ContainsValue(value));
                    Assert.IsTrue(value.Equals(clone[key]));
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

                Assert.AreEqual(hashtableType.Count, clone.Count);
                Assert.AreEqual(hashtableType.IsSynchronized, clone.IsSynchronized);
                Assert.AreEqual(hashtableType.IsFixedSize, clone.IsFixedSize);
                Assert.AreEqual(hashtableType.IsReadOnly, clone.IsReadOnly);

                for (int i = 0; i < clone.Count; i++)
                {
                    string key = "Key_" + i;
                    string value = "Value_" + i;

                    Assert.IsTrue(clone.Contains(key));
                    // TODO need ContainsValue
                    //Assert.IsNotNull(clone.ContainsValue(value));
                    Assert.IsTrue(value.Equals(clone[key]));
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
                Assert.AreEqual("Hello World", ((Foo)clone[i]).StringValue);
                Assert.AreSame(hash[i], clone[i]);
            }

            // Change object in original hashtable
            ((Foo)hash[1]).StringValue = "Goodbye";
            Assert.AreEqual("Goodbye", ((Foo)clone[1]).StringValue);

            // Removing an object from the original hashtable doesn't change the clone
            hash.Remove(0);
            Assert.IsTrue(clone.Contains(0));
        }

        [TestMethod]
        public void Clone_HashtableCastedToInterfaces()
        {
            // Try to cast the returned object from Clone() to different types
            Hashtable hash = CreateIntHashtable(100);

            ICollection collection = (ICollection)hash.Clone();
            Assert.AreEqual(hash.Count, collection.Count);

            IDictionary dictionary = (IDictionary)hash.Clone();
            Assert.AreEqual(hash.Count, dictionary.Count);
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

                    Assert.IsTrue(hashtableType.Contains(key), $"Expecting to find {key} key");
                }

                Assert.IsFalse(hashtableType.Contains("Non Existent Key"));

                Assert.IsFalse(hashtableType.Contains(101));
                Assert.IsFalse(hashtableType.Contains("Non Existent Key"));

                string removedKey = "Key_1";
                hashtableType.Remove(removedKey);

                Assert.IsFalse(hashtableType.Contains(removedKey), $"Hashtacble contains {removedKey} when it shouldn't.");
            }
        }

        [TestMethod]
        public void ContainsKey_NullKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(
                typeof(NullReferenceException),
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
                Assert.IsFalse(enumerator.Current.Equals("Key_0"));
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
                Assert.IsFalse(enumerator.Current.Equals("Value_0"));
            }
        }

        [TestMethod]
        public void Values_ModifyingHashtable_WhileEnumerating()
        {
            Assert.ThrowsException(typeof(InvalidOperationException),
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
            Assert.IsTrue(t.Contains(key1), $"Key '{key1}' not found in Hashtable.");
            Assert.IsTrue(t.Contains(key2), $"Key '{key2}' not found in Hashtable.");
            Assert.IsTrue(t.Contains(key3), $"Key '{key3}' not found in Hashtable.");
            Assert.IsTrue(t.Contains(key4), $"Key '{key4}' not found in Hashtable.");
            Assert.IsTrue(t.Contains(key5), $"Key '{key5}' not found in Hashtable.");

            // 3) check that the items are what they are expected to be
            // check the items reference and value first...
            int index = 0;

            foreach (string k in keys)
            {
                // test indexer
                MyClassTypeEntry entry = (MyClassTypeEntry)t[k];

                // check that the reference is the same 
                Assert.IsTrue(Object.ReferenceEquals(entry, entries[index]));

                // check that the values are the same
                Assert.IsTrue(entry.Equals(entries[index]), "Values don't match");

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

                Assert.IsTrue(found, $"Couldn't find '{k}' key.");
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
            Assert.IsTrue(t.Count == 0, "Hashtable shouldn't have elements, but it has.");

            foreach (string k in keys)
            {
                // test Contains
                Assert.IsFalse(t.Contains(k), $"Hashtable contains '{k}' key, but it shouldn't.");

                // test indexer
                MyClassTypeEntry entry = (MyClassTypeEntry)t[k];

                Assert.IsNull(entry, "'entry' should be null, but is not.");
            }
        }

        [TestMethod]
        public void Hashtable_Add_Clear()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            Assert.IsTrue(t.Count == vals.Length, "Hashtable and collection should have the same number of elements, but they don't.");

            t.Clear();

            Assert.IsTrue(t.Count == 0);

            ICollection keys = t.Keys;
            ICollection values = t.Values;

            Assert.IsTrue(keys.Count == 0, "Hashtable should have no keys, but it has.");

            Assert.IsTrue(values.Count == 0, "Hashtable should have no values, but it has.");
        }

        [TestMethod]
        public void Hashtable_CheckKeys()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            // check that the hastable contains the keys
            foreach (MyClassTypeEntry k in vals)
            {
                Assert.IsTrue(t.Contains(k.StringValue), $"Hashtable doesn't contain key '{k.StringValue}' but it should.");
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

                Assert.IsTrue(found, $"Collection doesn't contain key '{m.StringValue}' but it should.");
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
                Assert.IsTrue(t.Contains(k.StringValue), $"Hashtable doesn't contain element '{k.StringValue}' but it should.");
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

                Assert.IsTrue(verified, $"Collection doesn't contain element '{m.StringValue}' but it should.");
            }
        }

        [TestMethod]
        public void Hashtable_Count()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

            int count = t.Count;

            Assert.IsTrue(vals.Length == count);

            t.Add("a new key without a guid, can't exist", new MyClassTypeEntry());
            t.Add("a new key without a guid, can't exist again", new MyClassTypeEntry());
            t.Add("a new key without a guid, can't exist another time", new MyClassTypeEntry());

            Assert.IsTrue((count + 3) == t.Count, "Number of elements doesn't match.");
        }

        [TestMethod]
        public void Hashtable_Duplicate_01()
        {
            Hashtable t = new Hashtable();

            MyClassTypeEntry[] vals = InsertRandomValues(t, 10);

            // find a key and insert a duplicate: must fail
            MyClassTypeEntry entry = vals[vals.Length / 2];
            string key = MyClassTypeEntry.GetKey(entry.IntegerValue, entry.GuidValue);

            Assert.ThrowsException(typeof(ArgumentException), () =>
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

            Assert.ThrowsException(typeof(ArgumentException), () =>
            {
                ht.Add(duplicate, duplicate);

            }, "No exception was thrown when adding a duplicate element.");

            // remove the item 
            ht.Remove(duplicate);

            ht.Add(duplicate, duplicate);

            duplicate = ht.Count / 2;

            Assert.ThrowsException(typeof(ArgumentException), () =>
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

                Assert.IsTrue(masterKeyFound, $"Couldn't find key {masterKey} in the Array copy.");
            }

            // all keys should have been found 
            Assert.IsTrue(countOfKeysFound == t.Count, $"Keys count don't match. Hashtable has {t.Count} and was expecting {countOfKeysFound}");
        }

        [TestMethod]
        public void Hashtable_InsertNullKey()
        {
            Hashtable ht = CreateIntHashtable(10);

            Assert.ThrowsException(typeof(NullReferenceException), () =>
            {
                ht.Add(null, 9999);

            }, "No exception was thrown when adding a NULL key.");
        }

        [TestMethod]
        public void Hashtable_InsertClassAsKey()
        {
            Hashtable ht = new Hashtable();

            /////////////
            OutputHelper.WriteLine("Adding element with a 'string' as key.");

            ht.Add("Key 1", 9999);

            /////////////
            OutputHelper.WriteLine("Adding element with a 'class' as key.");

            ht.Add(new SomeKey(), 8888);

            /////////////
            OutputHelper.WriteLine("Adding element with another 'class' as key.");

            ht.Add(new SomeOtherKey(7777), 7777);
        }

        [TestMethod]
        public void Hashtable_ClassAsKey()
        {
            var hashtable = new Hashtable();

            OutputHelper.WriteLine("Adding 100 elements to the hashtable with SomeKey class");

            for (int i = 0; i < 100; i++)
            {
                SomeKey key = new SomeKey();

                hashtable.Add(key, key.GetHashCode());
            }

            foreach (var key in hashtable.Keys)
            {
                Assert.AreEqual(key.GetHashCode(), (int)hashtable[key], $"Failed on the {key} key.");
            }

            hashtable = new Hashtable();

            OutputHelper.WriteLine("Adding 100 elements to the hashtable with SomeOtherKey class");

            for (int i = 0; i < 100; i++)
            {
                SomeOtherKey key = new SomeOtherKey(i);

                hashtable.Add(key, key.I.GetHashCode());
            }

            foreach (var key in hashtable.Keys)
            {
                Assert.AreEqual((key as SomeOtherKey).I.GetHashCode(), (int)hashtable[key], $"Failed on the {key} key.");
            }
        }

        [TestMethod]
        public void Remove_AnotherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey key1 = new AnotherKey { id = 1, Value = 1.1 };
            AnotherKey key2 = new AnotherKey { id = 2, Value = 2.2 };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act
            hashtable.Remove(key1);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should still contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Remove_Foo_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo key1 = new Foo { StringValue = "foo1" };
            Foo key2 = new Foo { StringValue = "foo2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act
            hashtable.Remove(key1);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should still contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Remove_MyClassTypeEntry_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry key1 = new MyClassTypeEntry("string1", 1, Guid.NewGuid());
            MyClassTypeEntry key2 = new MyClassTypeEntry("string2", 2, Guid.NewGuid());
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act
            hashtable.Remove(key1);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should still contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Remove_SomeOtherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new();
            SomeOtherKey key1 = new SomeOtherKey(1);
            SomeOtherKey key2 = new SomeOtherKey(2);
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act
            hashtable.Remove(key1);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should still contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Remove_SomeKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key1" };
            SomeKey key2 = new SomeKey { KeyProperty = "key2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act
            hashtable.Remove(key1);

            // Assert
            Assert.IsFalse(hashtable.Contains(key1), "Hashtable should not contain key1 after removal.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should still contain key2.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Insert_AnotherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey key1 = new AnotherKey { id = 1, Value = 1.1 };
            AnotherKey key2 = new AnotherKey { id = 2, Value = 2.2 };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1 after insertion.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2 after insertion.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Insert_Foo_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo key1 = new Foo { StringValue = "foo1" };
            Foo key2 = new Foo { StringValue = "foo2" };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1 after insertion.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2 after insertion.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Insert_MyClassTypeEntry_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry key1 = new MyClassTypeEntry("string1", 1, Guid.NewGuid());
            MyClassTypeEntry key2 = new MyClassTypeEntry("string2", 2, Guid.NewGuid());
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1 after insertion.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2 after insertion.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Insert_SomeOtherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeOtherKey key1 = new SomeOtherKey(1);
            SomeOtherKey key2 = new SomeOtherKey(2);
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1 after insertion.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2 after insertion.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Insert_SomeKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key1" };
            SomeKey key2 = new SomeKey { KeyProperty = "key2" };
            string value1 = "value1";
            string value2 = "value2";

            // Act
            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1 after insertion.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2 after insertion.");
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
        }

        [TestMethod]
        public void Contains_AnotherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey key1 = new AnotherKey { id = 1, Value = 1.1 };
            AnotherKey key2 = new AnotherKey { id = 2, Value = 2.2 };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.IsFalse(hashtable.Contains(new AnotherKey { id = 3, Value = 3.3 }), "Hashtable should not contain a key that was not added.");
        }

        [TestMethod]
        public void Contains_Foo_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo key1 = new Foo { StringValue = "foo1" };
            Foo key2 = new Foo { StringValue = "foo2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.IsFalse(hashtable.Contains(new Foo { StringValue = "foo3" }), "Hashtable should not contain a key that was not added.");
        }

        [TestMethod]
        public void Contains_MyClassTypeEntry_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry key1 = new MyClassTypeEntry("string1", 1, Guid.NewGuid());
            MyClassTypeEntry key2 = new MyClassTypeEntry("string2", 2, Guid.NewGuid());
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.IsFalse(hashtable.Contains(new MyClassTypeEntry("string3", 3, Guid.NewGuid())), "Hashtable should not contain a key that was not added.");
        }

        [TestMethod]
        public void Contains_SomeOtherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeOtherKey key1 = new SomeOtherKey(1);
            SomeOtherKey key2 = new SomeOtherKey(2);
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.IsFalse(hashtable.Contains(new SomeOtherKey(3)), "Hashtable should not contain a key that was not added.");
        }

        [TestMethod]
        public void Contains_SomeKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key1" };
            SomeKey key2 = new SomeKey { KeyProperty = "key2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.IsTrue(hashtable.Contains(key1), "Hashtable should contain key1.");
            Assert.IsTrue(hashtable.Contains(key2), "Hashtable should contain key2.");
            Assert.IsFalse(hashtable.Contains(new SomeKey { KeyProperty = "key3" }), "Hashtable should not contain a key that was not added.");
        }

        [TestMethod]
        public void Accessor_AnotherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey key1 = new AnotherKey { id = 1, Value = 1.1 };
            AnotherKey key2 = new AnotherKey { id = 2, Value = 2.2 };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
            Assert.IsNull(hashtable[new AnotherKey { id = 3, Value = 3.3 }], "The value for a key that was not added should be null.");
        }

        [TestMethod]
        public void Accessor_Foo_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo key1 = new Foo { StringValue = "foo1" };
            Foo key2 = new Foo { StringValue = "foo2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
            Assert.IsNull(hashtable[new Foo { StringValue = "foo3" }], "The value for a key that was not added should be null.");
        }

        [TestMethod]
        public void Accessor_MyClassTypeEntry_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry key1 = new MyClassTypeEntry("string1", 1, Guid.NewGuid());
            MyClassTypeEntry key2 = new MyClassTypeEntry("string2", 2, Guid.NewGuid());
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
            Assert.IsNull(hashtable[new MyClassTypeEntry("string3", 3, Guid.NewGuid())], "The value for a key that was not added should be null.");
        }

        [TestMethod]
        public void Accessor_SomeOtherKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeOtherKey key1 = new SomeOtherKey(1);
            SomeOtherKey key2 = new SomeOtherKey(2);
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
            Assert.IsNull(hashtable[new SomeOtherKey(3)], "The value for a key that was not added should be null.");
        }

        [TestMethod]
        public void Accessor_SomeKey_AsKey()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey key1 = new SomeKey { KeyProperty = "key1" };
            SomeKey key2 = new SomeKey { KeyProperty = "key2" };
            string value1 = "value1";
            string value2 = "value2";

            hashtable.Add(key1, value1);
            hashtable.Add(key2, value2);

            // Act & Assert
            Assert.AreEqual(value1, hashtable[key1], "The value associated with key1 should be correct.");
            Assert.AreEqual(value2, hashtable[key2], "The value associated with key2 should be correct.");
            Assert.IsNull(hashtable[new SomeKey { KeyProperty = "key3" }], "The value for a key that was not added should be null.");
        }

        [TestMethod]
        public void ValuesCopyTo_IntArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < 10; i++)
            {
                hashtable.Add(i, i * 10);
            }

            int[] array = new int[10];

            // Act
            hashtable.Values.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the values, they may not be in the same order
            // a simplistic check is to sum the elements of both arrays and compare the sums

            int sum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += i * 10;
                arraySum += array[i];
            }

            Assert.AreEqual(sum, arraySum, "Sum of elements should be the same.");
        }

        [TestMethod]
        public void ValuesCopyTo_StringArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < 10; i++)
            {
                hashtable.Add(i.ToString(), (i * 10).ToString());
            }

            string[] array = new string[10];

            // Act
            hashtable.Values.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the values, they may not be in the same order
            // a simplistic check is to sum the hash code of elements on both arrays and compare the sums

            int sum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += (i * 10).ToString().GetHashCode();
                arraySum += array[i].GetHashCode();
            }

            Assert.AreEqual(sum, arraySum, "Sum of hash codes should be the same.");
        }

        [TestMethod]
        public void CopyTo_AnotherKeyArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            AnotherKey[] keys = new AnotherKey[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new AnotherKey { Value = i * 1.1 };
                hashtable.Add(keys[i], i * 10);
            }

            AnotherKey[] array = new AnotherKey[10];

            // Act
            hashtable.Keys.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the keys, they may not be in the same order
            // a simplistic check is to sum the elements of both arrays and compare the sums
            int keySum = 0;
            int arraySum = 0;
            for (int i = 0; i < 10; i++)
            {
                keySum += keys[i].id;
                arraySum += array[i].id;
            }

            Assert.AreEqual(keySum, arraySum, "Sum of elements should be the same.");
        }

        [TestMethod]
        public void CopyTo_FooArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            Foo[] keys = new Foo[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new Foo { StringValue = $"foo{i}" };
                hashtable.Add(keys[i], i * 10);
            }

            Foo[] array = new Foo[10];

            // Act
            hashtable.Keys.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the keys, they may not be in the same order
            // a simplistic check is to sum the hash code of elements of both arrays and compare the sums

            int keySum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                keySum += keys[i].GetHashCode();
                arraySum += array[i].GetHashCode();
            }

            Assert.AreEqual(keySum, arraySum, "Sum of hash codes should be the same.");
        }

        [TestMethod]
        public void CopyTo_MyClassTypeEntryArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            MyClassTypeEntry[] keys = new MyClassTypeEntry[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new MyClassTypeEntry($"string{i}", i, Guid.NewGuid());
                hashtable.Add(keys[i], i * 10);
            }

            MyClassTypeEntry[] array = new MyClassTypeEntry[10];

            // Act
            hashtable.Keys.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the keys, they may not be in the same order
            // a simplistic check is to sum the hash code of elements of both arrays and compare the sums

            int keySum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                keySum += keys[i].GetHashCode();
                arraySum += array[i].GetHashCode();
            }

            Assert.AreEqual(keySum, arraySum, "Sum of hash codes should be the same.");
        }

        [TestMethod]
        public void CopyTo_SomeOtherKeyArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeOtherKey[] keys = new SomeOtherKey[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new SomeOtherKey(i);
                hashtable.Add(keys[i], i * 10);
            }

            SomeOtherKey[] array = new SomeOtherKey[10];

            // Act
            hashtable.Keys.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the keys, they may not be in the same order
            // a simplistic check is to sum the hash code of elements of both arrays and compare the sums

            int keySum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                keySum += keys[i].GetHashCode();
                arraySum += array[i].GetHashCode();
            }

            Assert.AreEqual(keySum, arraySum, "Sum of hash codes should be the same.");
        }

        [TestMethod]
        public void CopyTo_SomeKeyArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeKey[] keys = new SomeKey[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new SomeKey { KeyProperty = $"key{i}" };
                hashtable.Add(keys[i], i * 10);
            }

            SomeKey[] array = new SomeKey[10];

            // Act
            hashtable.Keys.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the keys, they may not be in the same order
            // a simplistic check is to sum the hash code of elements of both arrays and compare the sums

            int keySum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                keySum += keys[i].GetHashCode();
                arraySum += array[i].GetHashCode();
            }

            Assert.AreEqual(keySum, arraySum, "Sum of hash codes should be the same.");
        }

        [TestMethod]
        public void CopyTo_SomeOtherArray()
        {
            // Arrange
            Hashtable hashtable = new Hashtable();
            SomeOtherKey[] keys = new SomeOtherKey[10];
            for (int i = 0; i < 10; i++)
            {
                keys[i] = new SomeOtherKey(i);
                hashtable.Add(keys[i], i * 10);
            }

            DictionaryEntry[] array = new DictionaryEntry[10];

            // Act
            hashtable.CopyTo(array, 0);

            // Assert
            // despite array elements should be the same as the hash table elements, they may not be in the same order
            // a simplistic check is to sum the values of elements of both arrays and compare the sums
            // at the same time, checking if the hash table contains the key

            int keySum = 0;
            int arraySum = 0;

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(hashtable.Contains(array[i].Key), "Hashtable should contain the key.");
                Assert.AreEqual(hashtable[array[i].Key], array[i].Value, "The value associated with the key should be correct.");

                keySum += i * 10;
                arraySum += (int)hashtable[array[i].Key];
            }

            Assert.AreEqual(keySum, arraySum, "Sum of value should be the same.");
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
                Assert.AreEqual(0, hash1.Count, "Element count is different.");
            }
            else
            {
                // Make sure that construtor imports all keys and values
                Assert.AreEqual(hash2.Count, hash1.Count);

                for (int i = 0; i < 100; i++)
                {
                    Assert.IsTrue(hash1.Contains(i));
                    Assert.IsNotNull(hash1[i]);
                }

                // Make sure the new and old hashtables are not linked
                hash2.Clear();

                for (int i = 0; i < 100; i++)
                {
                    Assert.IsTrue(hash1.Contains(i));
                    Assert.IsNotNull(hash1[i]);
                }
            }

            Assert.IsFalse(hash1.IsFixedSize);
            Assert.IsFalse(hash1.IsReadOnly);
            Assert.IsFalse(hash1.IsSynchronized);

            // Make sure we can add to the hashtable
            int count = hash1.Count;
            for (int i = count; i < count + 100; i++)
            {
                hash1.Add(i, i);

                Assert.IsTrue(hash1.Contains(i));
                Assert.IsNotNull(hash1[i]);
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

        public class EmptyKey
        {
        }


        public class SomeOtherKey
        {
            private readonly int _i;
            public int I { get; }

            public SomeOtherKey(int i)
            {
                _i = i;
            }
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

        public class AnotherKey
        {
            private static int idProvider = 31173;
            public int id;
            private double _value = 0;

            public double Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public AnotherKey()
            {
                id = idProvider++;
            }

            public override int GetHashCode()
            {
                return id;
            }

            public override bool Equals(object obj)
            {
                return obj is AnotherKey component
                       && component.id == id;
            }
        }
        class SomeKey
        {
            public string KeyProperty { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is SomeKey otherKey)
                {
                    return KeyProperty == otherKey.KeyProperty;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (KeyProperty != null) ? KeyProperty.GetHashCode() : 0;
            }
        }

        #endregion
    }
}
