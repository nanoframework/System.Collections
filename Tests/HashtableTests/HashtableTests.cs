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
        private const int c_BareMinimum    = 1;

        //--//

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
                catch(Exception e)
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

        //Test Case Calls 
        [TestMethod]
        public void Hashtable_Add_Contains_Remove_Test1()
        {
            // Enter 5 values with 5 unique keys
            // 1) check that the keys are in the table
            // 2) check that all keys are present
            // 3) check that the items are what they are expected to be through all possible interfaces
            // 4) check that we can successfully remove the items 
            // 5) check that a removed item is no longer in the table, and so its key it is no longer in the table as well

            try
            {
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

                foreach (String k in keys)
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
                foreach (String k in keys)
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
                foreach (String k in keys)
                {
                    t.Remove(k);
                }

                // 4) checked that we can remove the items
                // ... then check the keys                
                foreach (String k in keys)
                {
                    t.Remove(k);
                }

                // 5) check that a removed item is no longer in the table, and so its key it is no longer in the table as well           
                // check the items reference and value first...
                // test nothing is left in the Hashtable 
                Assert.True(t.Count == 0, "Hashtable shouldn't have elements, but it has.");

                foreach (String k in keys)
                {
                    // test Contains
                    Assert.False(t.Contains(k), $"Hashtable contains '{k}' key, but it shouldn't.");

                    // test indexer
                    MyClassTypeEntry entry = (MyClassTypeEntry)t[k];

                    Assert.Null(entry, "'entry' should be null, but is not.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected exception", e);
            }
        }
        
        [TestMethod]
        public void Hashtable_Add_Clear()
        {
            try
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
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }
        
        [TestMethod]
        public void Hashtable_CheckKeys()
        {
            try
            {
                Hashtable t = new Hashtable();

                MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

                // check that the hastable contains the keys
                foreach (MyClassTypeEntry k in vals)
                {
                    Assert.True(t.Contains(k.StringValue), $"Hashtable doesn't contain key '{k.StringValue}' but it should.");
                }

                ICollection keys = t.Keys;

                foreach(MyClassTypeEntry m in vals)
                {
                    // check that the key collection contains the key
                    bool found = false;

                    foreach(string s in keys)
                    {
                        if(m.StringValue.Equals(s))
                        {
                            found = true; break;
                        }
                    }

                    Assert.True(found, $"Collection doesn't contain key '{m.StringValue}' but it should.");
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void Hashtable_CheckValues()
        {
            try
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
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void Hashtable_Count()
        {
            try
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
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void Hashtable_Duplicate()
        {
            try
            {
                Hashtable t = new Hashtable();

                MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

                //
                // find a key and insert a duplicate: must fail
                //
                MyClassTypeEntry entry = vals[vals.Length / 2];
                string key = MyClassTypeEntry.GetKey(entry.IntegerValue, entry.GuidValue);

                bool exceptionThrown = false;
                try
                {
                    t.Add(key, new MyClassTypeEntry());
                }
                catch(Exception e)
                {
                    // Expected exception -- duplicate in Hashtable
                    exceptionThrown = true;
                }

                Assert.True(exceptionThrown, "No exception was thrown when adding a duplicate element.");

                // remove the item 
                t.Remove(key);

                // try insert again: must succeed
                exceptionThrown = false;
                
                try
                {
                    t.Add(key, new MyClassTypeEntry());
                }
                catch
                {
                    // NO exception is expected
                    exceptionThrown = true;
                }

                Assert.False(exceptionThrown, "Exception not expected when adding element.");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

         [TestMethod]
         public void Hashtable_CopyTo()
         {
             try
             {
                Hashtable t = new Hashtable();

                MyClassTypeEntry[] vals = InsertRandomValues(t, c_MinimumEntries);

                int[] keys = new int[5];
                t.Keys.CopyTo(keys, 0);

                int countOfKeysFound = 0;

                // check that all keys have been copied
                foreach (var masterKey in t.Keys)
                {
                    bool masterKeyFound = false;

                    foreach (var key in keys)
                    {
                        if (key == (int)masterKey)
                        {
                            masterKeyFound = true;

                            countOfKeysFound++;

                            break;
                        }
                    }

                    Assert.True(masterKeyFound, $"Couldn't find key {masterKey} in the Array copy.");
                }

                // all keys should have been found 
                Assert.True(countOfKeysFound == t.Count);
            }
            catch (Exception e)
             {
                 new Exception("Unexpected exception", e);
             }
         }

         [TestMethod]
         public void Hashtable_Clone()
         {
             try
             {
                 // TODO TODO TODO 
             }
             catch (Exception e)
             {
                 new Exception("Unexpected exception", e);
             }
         }

        //--//

        /// <summary>
        /// Creates a MyClassEntry type whose string member with the prefix is the key item in the hashtable
        /// </summary>
        private MyClassTypeEntry[] InsertRandomValues(Hashtable t, int max)
        {
            int count = (new Random().Next() % max) + c_BareMinimum; // at least 1

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
    }
}
