// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GenericCollections
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void List_Constructor_Default()
        {
            var listInt = new List<int>();
            Assert.IsNotNull(listInt);
            Assert.AreEqual(0, listInt.Count);

            var listString = new List<string>();
            Assert.IsNotNull(listString);
            Assert.AreEqual(0, listString.Count);

            var listObject = new List<object>();
            Assert.IsNotNull(listObject);
            Assert.AreEqual(0, listObject.Count);

            var listClass = new List<DummyClass>();
            Assert.IsNotNull(listClass);
            Assert.AreEqual(0, listClass.Count);
        }

        [TestMethod]
        public void List_Constructor_Capacity()
        {
            var listInt = new List<int>(10);
            Assert.IsNotNull(listInt);
            Assert.AreEqual(0, listInt.Count);

            var listString = new List<string>(5);
            Assert.IsNotNull(listString);
            Assert.AreEqual(0, listString.Count);

            var listObject = new List<object>(20);
            Assert.IsNotNull(listObject);
            Assert.AreEqual(0, listObject.Count);

            var listClass = new List<DummyClass>(15);
            Assert.IsNotNull(listClass);
            Assert.AreEqual(0, listClass.Count);
        }

        [TestMethod]
        public void List_Constructor_IEnumerable()
        {
            int[] initialInt = new int[] { 1, 2, 3 };
            List<int> listInt = new List<int>(initialInt);
            Assert.IsNotNull(listInt);
            Assert.AreEqual(3, listInt.Count);
            Assert.AreEqual(1, listInt[0]);
            Assert.AreEqual(2, listInt[1]);
            Assert.AreEqual(3, listInt[2]);

            string[] initialString = new string[] { "A", "B", "C" };
            List<string> listString = new List<string>(initialString);
            Assert.IsNotNull(listString);
            Assert.AreEqual(3, listString.Count);
            Assert.AreEqual("A", listString[0]);
            Assert.AreEqual("B", listString[1]);
            Assert.AreEqual("C", listString[2]);

            DummyClass[] initialClass = new DummyClass[]
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
                };
            List<DummyClass> listClass = new List<DummyClass>(initialClass);
            Assert.IsNotNull(listClass);
            Assert.AreEqual(3, listClass.Count);
            Assert.AreEqual(1, listClass[0].Id);
            Assert.AreEqual("Two", listClass[1].Name);
            Assert.AreEqual(3, listClass[2].Id);
        }

        [TestMethod]
        public void List_Add_And_Count()
        {
            var listInt = new List<int>();
            listInt.Add(1);
            listInt.Add(10);
            Assert.AreEqual(2, listInt.Count);

            var listString = new List<string>();
            listString.Add("Hello");
            listString.Add("World");
            Assert.AreEqual(2, listString.Count);

            var listObject = new List<object>();
            listObject.Add(new object());
            listObject.Add(new object());
            Assert.AreEqual(2, listObject.Count);

            var listClass = new List<DummyClass>();
            listClass.Add(new DummyClass(1, "Test1"));
            listClass.Add(new DummyClass(2, "Test2"));
            Assert.AreEqual(2, listClass.Count);
        }

        [TestMethod]
        public void List_Indexer_Get_And_Set()
        {
            var listInt = new List<int> { 1, 2, 3 };
            Assert.AreEqual(1, listInt[0]);
            Assert.AreEqual(2, listInt[1]);
            Assert.AreEqual(3, listInt[2]);
            listInt[1] = 20;
            Assert.AreEqual(20, listInt[1]);

            var listString = new List<string> { "A", "B", "C" };
            Assert.AreEqual("A", listString[0]);
            Assert.AreEqual("B", listString[1]);
            Assert.AreEqual("C", listString[2]);
            listString[1] = "Z";
            Assert.AreEqual("Z", listString[1]);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
            };

            Assert.AreEqual(1, listClass[0].Id);
            Assert.AreEqual("Two", listClass[1].Name);
            Assert.AreEqual(3, listClass[2].Id);

            listClass[1] = new DummyClass(20, "Twenty");
            Assert.AreEqual(20, listClass[1].Id);
            Assert.AreEqual("Twenty", listClass[1].Name);
        }

        [TestMethod]
        public void List_Clear()
        {
            var listInt = new List<int> { 1, 2, 3 };
            Assert.AreEqual(3, listInt.Count);
            listInt.Clear();
            Assert.AreEqual(0, listInt.Count);

            var listString = new List<string> { "A", "B", "C" };
            Assert.AreEqual(3, listString.Count);
            listString.Clear();
            Assert.AreEqual(0, listString.Count);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two")
            };

            Assert.AreEqual(2, listClass.Count);
            listClass.Clear();
            Assert.AreEqual(0, listClass.Count);
        }

        [TestMethod]
        public void List_Contains()
        {
            var listInt = new List<int> { 1, 2, 3 };
            Assert.IsTrue(listInt.Contains(2));
            Assert.IsFalse(listInt.Contains(5));

            var listString = new List<string> { "A", "B", "C" };
            Assert.IsTrue(listString.Contains("B"));
            Assert.IsFalse(listString.Contains("Z"));

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two")
            };
            var existingItem = listClass[0];
            var nonExistingItem = new DummyClass(3, "Three");
            Assert.IsTrue(listClass.Contains(existingItem));
            Assert.IsFalse(listClass.Contains(nonExistingItem));
        }

        [TestMethod]
        public void List_Remove()
        {
            var listInt = new List<int> { 1, 2, 3 };
            Assert.IsTrue(listInt.Remove(2));
            Assert.AreEqual(2, listInt.Count);
            Assert.IsFalse(listInt.Remove(5));
            Assert.AreEqual(2, listInt.Count);

            var listString = new List<string> { "A", "B", "C" };
            Assert.IsTrue(listString.Remove("B"));
            Assert.AreEqual(2, listString.Count);
            Assert.IsFalse(listString.Remove("Z"));
            Assert.AreEqual(2, listString.Count);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
            };

            var itemToRemove = listClass[1];
            Assert.IsTrue(listClass.Remove(itemToRemove));
            Assert.AreEqual(2, listClass.Count);

            var nonExistingItem = new DummyClass(4, "Four");
            Assert.IsFalse(listClass.Remove(nonExistingItem));
            Assert.AreEqual(2, listClass.Count);
        }

        [TestMethod]
        public void List_Insert()
        {
            var listInt = new List<int> { 1, 3 };
            listInt.Insert(1, 2);

            Assert.AreEqual(3, listInt.Count);
            Assert.AreEqual(1, listInt[0]);
            Assert.AreEqual(2, listInt[1]);
            Assert.AreEqual(3, listInt[2]);

            var listString = new List<string> { "A", "C" };
            listString.Insert(1, "B");

            Assert.AreEqual(3, listString.Count);
            Assert.AreEqual("A", listString[0]);
            Assert.AreEqual("B", listString[1]);
            Assert.AreEqual("C", listString[2]);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
            };
            var newItem = new DummyClass(1, "One and a Half");
            listClass.Insert(1, newItem);
            Assert.AreEqual(4, listClass.Count);
            Assert.AreEqual(1, listClass[0].Id);
            Assert.AreEqual("One and a Half", listClass[1].Name);
            Assert.AreEqual(2, listClass[2].Id);
            Assert.AreEqual(3, listClass[3].Id);
        }

        [TestMethod]
        public void List_IndexOf()
        {
            var listInt = new List<int> { 1, 2, 3 };
            Assert.AreEqual(1, listInt.IndexOf(2));
            Assert.AreEqual(-1, listInt.IndexOf(5));

            var listString = new List<string> { "A", "B", "C" };
            Assert.AreEqual(1, listString.IndexOf("B"));
            Assert.AreEqual(-1, listString.IndexOf("Z"));

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
                };
            var existingItem = listClass[1];
            var nonExistingItem = new DummyClass(4, "Four");
            Assert.AreEqual(1, listClass.IndexOf(existingItem));
            Assert.AreEqual(-1, listClass.IndexOf(nonExistingItem));
        }

        [TestMethod]
        public void List_RemoveAt()
        {
            var listInt = new List<int> { 1, 2, 3 };
            listInt.RemoveAt(1);
            Assert.AreEqual(2, listInt.Count);
            Assert.AreEqual(1, listInt[0]);
            Assert.AreEqual(3, listInt[1]);

            var listString = new List<string> { "A", "B", "C" };
            listString.RemoveAt(1);
            Assert.AreEqual(2, listString.Count);
            Assert.AreEqual("A", listString[0]);
            Assert.AreEqual("C", listString[1]);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
                };
            listClass.RemoveAt(1);
            Assert.AreEqual(2, listClass.Count);
            Assert.AreEqual(1, listClass[0].Id);
            Assert.AreEqual(3, listClass[1].Id);
        }

        [TestMethod]
        public void List_ToArray()
        {
            var listInt = new List<int> { 1, 2, 3 };
            var arrayInt = listInt.ToArray();
            Assert.AreEqual(3, arrayInt.Length);
            Assert.AreEqual(1, arrayInt[0]);
            Assert.AreEqual(2, arrayInt[1]);
            Assert.AreEqual(3, arrayInt[2]);

            var listString = new List<string> { "A", "B", "C" };
            var arrayString = listString.ToArray();
            Assert.AreEqual(3, arrayString.Length);
            Assert.AreEqual("A", arrayString[0]);
            Assert.AreEqual("B", arrayString[1]);
            Assert.AreEqual("C", arrayString[2]);

            var listClass = new List<DummyClass>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
            };
            var arrayClass = listClass.ToArray();
            Assert.AreEqual(3, arrayClass.Length);
            Assert.AreEqual(1, arrayClass[0].Id);
            Assert.AreEqual("Two", arrayClass[1].Name);
            Assert.AreEqual(3, arrayClass[2].Id);
        }

        [TestMethod]
        public void List_Constructor_Capacity_Zero()
        {
            var list = new List<int>(0);
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);

            // Should still be able to add items
            list.Add(1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0]);
        }

        [TestMethod]
        public void List_Constructor_Capacity_Negative_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                     {
                         _ = new List<int>(-1);
                     });
        }

        [TestMethod]
        public void List_Constructor_IEnumerable_Empty()
        {
            var emptyArray = new int[] { };
            var list = new List<int>(emptyArray);
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void List_Constructor_IEnumerable_Null_ThrowsException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () =>
            {
                var list = new List<int>(null);
            });
        }

        [TestMethod]
        public void List_Indexer_Get_OutOfRange_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
               {
                   _ = list[5];
               });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
            {
                _ = list[-1];
            });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
            {
                _ = list[3];
            });
        }

        [TestMethod]
        public void List_Indexer_Set_OutOfRange_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
            {
                list[5] = 10;
            });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
            {
                list[-1] = 10;
            });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
             {
                 list[3] = 10;
             });
        }

        [TestMethod]
        public void List_Add_AfterClear()
        {
            var list = new List<int> { 1, 2, 3 };
            list.Clear();
            list.Add(10);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(10, list[0]);

            list.Add(20);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(20, list[1]);
        }

        [TestMethod]
        public void List_Insert_AtBeginning()
        {
            var list = new List<int> { 2, 3 };
            list.Insert(0, 1);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        [TestMethod]
        public void List_Insert_AtEnd()
        {
            var list = new List<int> { 1, 2 };
            list.Insert(2, 3);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        [TestMethod]
        public void List_Insert_OutOfRange_ThrowsException()
        {
            var list = new List<int> { 1, 2 };

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                 {
                     list.Insert(5, 10);
                 });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                {
                    list.Insert(-1, 10);
                });
        }

        [TestMethod]
        public void List_RemoveAt_OutOfRange_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                {
                    list.RemoveAt(5);
                });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                {
                    list.RemoveAt(-1);
                });

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
                {
                    list.RemoveAt(3);
                });
        }

        [TestMethod]
        public void List_RemoveAt_FirstElement()
        {
            var list = new List<int> { 1, 2, 3 };
            list.RemoveAt(0);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);
        }

        [TestMethod]
        public void List_RemoveAt_LastElement()
        {
            var list = new List<int> { 1, 2, 3 };
            list.RemoveAt(2);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
        }

        [TestMethod]
        public void List_Contains_EmptyList()
        {
            var list = new List<int>();
            Assert.IsFalse(list.Contains(1));

            var listString = new List<string>();
            Assert.IsFalse(listString.Contains("test"));
        }

        [TestMethod]
        public void List_Contains_NullValue()
        {
            var list = new List<string> { "A", null, "B" };
            Assert.IsTrue(list.Contains(null));

            var listWithoutNull = new List<string> { "A", "B" };
            Assert.IsFalse(listWithoutNull.Contains(null));
        }

        [TestMethod]
        public void List_IndexOf_MultipleOccurrences()
        {
            var list = new List<int> { 1, 2, 3, 2, 4 };
            // Should return first occurrence
            Assert.AreEqual(1, list.IndexOf(2));

            var listString = new List<string> { "A", "B", "C", "B", "D" };
            Assert.AreEqual(1, listString.IndexOf("B"));
        }

        [TestMethod]
        public void List_IndexOf_EmptyList()
        {
            var list = new List<int>();
            Assert.AreEqual(-1, list.IndexOf(1));

            var listString = new List<string>();
            Assert.AreEqual(-1, listString.IndexOf("test"));
        }

        [TestMethod]
        public void List_Remove_FirstOccurrence()
        {
            var list = new List<int> { 1, 2, 3, 2, 4 };
            Assert.IsTrue(list.Remove(2));
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(3, list[1]); // Second 2 moved to position 1
            Assert.AreEqual(2, list[2]); // But one 2 still remains at position 2
        }

        [TestMethod]
        public void List_Remove_FromEmptyList()
        {
            var list = new List<int>();
            Assert.IsFalse(list.Remove(1));
        }

        [TestMethod]
        public void List_ToArray_EmptyList()
        {
            var list = new List<int>();
            var array = list.ToArray();
            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);

            var listString = new List<string>();
            var arrayString = listString.ToArray();
            Assert.IsNotNull(arrayString);
            Assert.AreEqual(0, arrayString.Length);
        }

        [TestMethod]
        public void List_ToArray_Modification()
        {
            var list = new List<int> { 1, 2, 3 };
            var array = list.ToArray();

            // Modifying the array should not affect the list
            array[0] = 100;
            Assert.AreEqual(1, list[0]);

            // Modifying the list should not affect the array
            list[0] = 200;
            Assert.AreEqual(100, array[0]);
        }

        [TestMethod]
        public void List_Capacity_GetSet()
        {
            var list = new List<int>(10);
            Assert.IsTrue(list.Capacity >= 10);

            list.Add(1);
            list.Add(2);

            // Set capacity to larger value
            list.Capacity = 20;
            Assert.AreEqual(20, list.Capacity);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
        }

        [TestMethod]
        public void List_Capacity_SetLessThanCount_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () =>
               {
                   list.Capacity = 3;
               });
        }

        [TestMethod]
        public void List_Enumerator_Basic()
        {
            var list = new List<int> { 1, 2, 3 };
            int sum = 0;
            int count = 0;

            foreach (int item in list)
            {
                sum += item;
                count++;
            }

            Assert.AreEqual(6, sum);
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void List_Enumerator_EmptyList()
        {
            var list = new List<int>();
            int count = 0;

            foreach (int item in list)
            {
                count++;
            }

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void List_Enumerator_ModificationDuringEnumeration_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            Assert.ThrowsException(typeof(InvalidOperationException), () =>
                {
                    foreach (int item in list)
                    {
                        list.Add(10); // Modifying during enumeration
                    }
                });
        }

        [TestMethod]
        public void List_CopyTo_Array()
        {
            var list = new List<int> { 1, 2, 3 };
            int[] array = new int[5];

            list.CopyTo(array, 1);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(0, array[4]);
        }

        [TestMethod]
        public void List_CopyTo_WithIndex()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            int[] array = new int[3];

            list.CopyTo(1, array, 0, 3);

            Assert.AreEqual(2, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
        }

        [TestMethod]
        public void List_ForEach_Action()
        {
            var list = new List<int> { 1, 2, 3 };
            int sum = 0;

            list.ForEach(item => sum += item);

            Assert.AreEqual(6, sum);
        }

        [TestMethod]
        public void List_ForEach_NullAction_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };

            Assert.ThrowsException(typeof(ArgumentNullException), () =>
                {
                    list.ForEach(null);
                });
        }

        [TestMethod]
        public void List_ForEach_ModificationDuringForEach_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };

            Assert.ThrowsException(typeof(InvalidOperationException), () =>
               {
                   list.ForEach(item => list.Add(10));
               });
        }

        [TestMethod]
        public void List_MultipleOperations_Sequence()
        {
            var list = new List<int>();

            // Add items
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }
            Assert.AreEqual(10, list.Count);

            // Insert in middle
            list.Insert(5, 100);
            Assert.AreEqual(11, list.Count);
            Assert.AreEqual(100, list[5]);

            // Remove specific item
            list.Remove(100);
            Assert.AreEqual(10, list.Count);

            // RemoveAt
            list.RemoveAt(0);
            Assert.AreEqual(9, list.Count);
            Assert.AreEqual(1, list[0]);

            // Clear
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void List_WithReferenceTypes_Nulls()
        {
            var list = new List<string>();
            list.Add(null);
            list.Add("test");
            list.Add(null);

            Assert.AreEqual(3, list.Count);
            Assert.IsNull(list[0]);
            Assert.AreEqual("test", list[1]);
            Assert.IsNull(list[2]);

            Assert.IsTrue(list.Contains(null));
            Assert.AreEqual(0, list.IndexOf(null));

            list.Remove(null);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("test", list[0]);
        }

        [TestMethod]
        public void List_IList_Interface()
        {
            IList list = new List<int> { 1, 2, 3 };

            Assert.AreEqual(3, list.Count);
            Assert.IsFalse(list.IsFixedSize);
            Assert.IsFalse(list.IsReadOnly);
            Assert.IsFalse(list.IsSynchronized);

            Assert.AreEqual(1, list[0]);
            list[0] = 10;
            Assert.AreEqual(10, list[0]);

            list.Add(4);
            Assert.AreEqual(4, list.Count);

            list.Remove(10);
            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.Contains(2));
            Assert.AreEqual(0, list.IndexOf(2));

            list.Insert(0, 100);
            Assert.AreEqual(100, list[0]);

            list.RemoveAt(0);
            Assert.AreEqual(2, list[0]);

            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void List_LargeCapacity_Growth()
        {
            var list = new List<int>();

            // Add many items to test capacity growth
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            Assert.AreEqual(100, list.Count);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, list[i]);
            }
        }
    }

    internal class DummyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DummyClass(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
