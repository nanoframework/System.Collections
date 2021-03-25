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
    public class QueueTests
    {
        //Test Case Calls 
        [TestMethod]
        public void EnqueueDequeuePeekTest1()
        {
            try
            {
                Queue queue = new Queue();

                queue.Enqueue(TestObjects.u1);
                queue.Enqueue(TestObjects.s1);
                queue.Enqueue(TestObjects.u2);
                queue.Enqueue(TestObjects.s2);
                queue.Enqueue(TestObjects.u4);
                queue.Enqueue(TestObjects.s4);
                queue.Enqueue(TestObjects.u8);
                queue.Enqueue(TestObjects.s8);
                queue.Enqueue(TestObjects.f4);
                queue.Enqueue(TestObjects.f8);
                queue.Enqueue(TestObjects.c2);
                queue.Enqueue(TestObjects.str);
                queue.Enqueue(TestObjects.dt);
                queue.Enqueue(TestObjects.ts);
                queue.Enqueue(TestObjects.st);
                queue.Enqueue(TestObjects.cl);
                queue.Enqueue(TestObjects.o);
                queue.Enqueue(TestObjects.nul);
                queue.Enqueue(TestObjects.en);

                Assert.True(19 == queue.Count, "Queue.Count is incorrect");

                Assert.True(TestObjects.u1.Equals(queue.Peek()));
                Assert.True(TestObjects.u1.Equals(queue.Dequeue()));
                Assert.True(TestObjects.s1.Equals(queue.Peek()));
                Assert.True(TestObjects.s1.Equals(queue.Dequeue()));
                Assert.True(TestObjects.u2.Equals(queue.Peek()));
                Assert.True(TestObjects.u2.Equals(queue.Dequeue()));
                Assert.True(TestObjects.s2.Equals(queue.Peek()));
                Assert.True(TestObjects.s2.Equals(queue.Dequeue()));
                Assert.True(TestObjects.u4.Equals(queue.Peek()));
                Assert.True(TestObjects.u4.Equals(queue.Dequeue()));
                Assert.True(TestObjects.s4.Equals(queue.Peek()));
                Assert.True(TestObjects.s4.Equals(queue.Dequeue()));
                Assert.True(TestObjects.u8.Equals(queue.Peek()));
                Assert.True(TestObjects.u8.Equals(queue.Dequeue()));
                Assert.True(TestObjects.s8.Equals(queue.Peek()));
                Assert.True(TestObjects.s8.Equals(queue.Dequeue()));
                Assert.True(TestObjects.f4.Equals(queue.Peek()));
                Assert.True(TestObjects.f4.Equals(queue.Dequeue()));
                Assert.True(TestObjects.f8.Equals(queue.Peek()));
                Assert.True(TestObjects.f8.Equals(queue.Dequeue()));
                Assert.True(TestObjects.c2.Equals(queue.Peek()));
                Assert.True(TestObjects.c2.Equals(queue.Dequeue()));
                Assert.True(TestObjects.str.Equals(queue.Peek()));
                Assert.True(TestObjects.str.Equals(queue.Dequeue()));
                Assert.True(TestObjects.dt.Equals(queue.Peek()));
                Assert.True(TestObjects.dt.Equals(queue.Dequeue()));
                Assert.True(TestObjects.ts.Equals(queue.Peek()));
                Assert.True(TestObjects.ts.Equals(queue.Dequeue()));
                Assert.True(TestObjects.st.Equals(queue.Peek()));
                Assert.True(TestObjects.st.Equals(queue.Dequeue()));
                Assert.True(TestObjects.cl.Equals(queue.Peek()));
                Assert.True(TestObjects.cl.Equals(queue.Dequeue()));
                Assert.True(TestObjects.o.Equals(queue.Peek()));
                Assert.True(TestObjects.o.Equals(queue.Dequeue()));
                Assert.True(TestObjects.nul.Equals(queue.Peek()));
                Assert.True(TestObjects.nul.Equals(queue.Dequeue()));
                Assert.True(TestObjects.en.Equals(queue.Peek()));
                Assert.True(TestObjects.en.Equals(queue.Dequeue()));

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void EnqueueDequeuePeekTest2()
        {
            try
            {
                Queue queue = new Queue();

                for (int i = 0; i < 8; i++)
                {
                    queue.Enqueue(i);
                }

                Assert.True(8 == queue.Count, "Queue.Count is incorrect");

                for (int i = 0; i < 4; i++)
                {
                    Assert.True(i.Equals(queue.Peek()));
                    Assert.True(i.Equals(queue.Dequeue()));
                }

                Assert.True(4 == queue.Count, "Queue.Count is incorrect");

                for (int i = 8; i < 12; i++)
                {
                    queue.Enqueue(i);
                }

                Assert.True(8 == queue.Count, "Queue.Count is incorrect");

                for (int i = 4; i < 6; i++)
                {
                    Assert.True(i.Equals(queue.Peek()));
                    Assert.True(i.Equals(queue.Dequeue()));
                }

                Assert.True(6 == queue.Count, "Queue.Count is incorrect");

                for (int i = 12; i < 16; i++)
                {
                    queue.Enqueue(i);
                }

                Assert.True(10 == queue.Count, "Queue.Count is incorrect");

                for (int i = 6; i < 16; i++)
                {
                    Assert.True(i.Equals(queue.Peek()));
                    Assert.True(i.Equals(queue.Dequeue()));
                }

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void EnqueueDequeueNullTest()
        {
            try
            {
                Queue queue = new Queue();

                for (int i = 0; i < 20; i++)
                {
                    queue.Enqueue(null);
                }

                Assert.True(20 == queue.Count, "Queue.Count is incorrect");

                for (int i = 0; i < 20; i++)
                {
                    Assert.Null(queue.Dequeue());
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void ClearTest()
        {
            try
            {
                Queue queue = BuildNormalQueue();

                queue.Clear();

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");

                queue = BuildWrappedAroundQueue();

                queue.Clear();

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void CloneTest()
        {
            try
            {
                Queue queue = BuildNormalQueue();
                Queue clone = (Queue)queue.Clone();

                Assert.True(6 == queue.Count, "Queue.Count is incorrect");
                Assert.True(6 == clone.Count, "Queue.Count is incorrect");

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(queue.Dequeue()));
                }

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");
                Assert.True(6 == clone.Count, "Queue.Count is incorrect");

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(clone.Dequeue()));
                }


                queue = BuildWrappedAroundQueue();
                clone = (Queue)queue.Clone();

                Assert.True(6 == queue.Count, "Queue.Count is incorrect");
                Assert.True(6 == clone.Count, "Queue.Count is incorrect");

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(queue.Dequeue()));
                }

                Assert.True(0 == queue.Count, "Queue.Count is incorrect");
                Assert.True(6 == clone.Count, "Queue.Count is incorrect");

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(clone.Dequeue()));
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void CopyToTest()
        {
            try
            {
                Queue queue = BuildNormalQueue();
                int[] intArray = new int[10];
                Object[] objArray = new Object[10];

                queue.CopyTo(intArray, 4);
                queue.CopyTo(objArray, 4);

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(intArray[i]));
                    Assert.True(i.Equals((int)objArray[i]));
                }

                queue = BuildWrappedAroundQueue();
                intArray = new int[10];
                objArray = new Object[10];

                queue.CopyTo(intArray, 4);
                queue.CopyTo(objArray, 4);

                for (int i = 4; i < 10; i++)
                {
                    Assert.True(i.Equals(intArray[i]));
                    Assert.True(i.Equals((int)objArray[i]));
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void EnumeratorTest()
        {
            try
            {
                Queue queue = BuildNormalQueue();

                int j = 4;

                foreach (int i in queue)
                {
                    Assert.True(j++ == i);
                }

                queue = BuildWrappedAroundQueue();

                j = 4;

                foreach (int i in queue)
                {
                    Assert.True(j++ == i);
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void ContainsTest()
        {
            try
            {
                Queue queue = BuildNormalQueue();
                int i;

                for (i = 0; i < 4; i++)
                {
                    Assert.True(queue.Contains(i));
                }

                for (; i < 10; i++)
                {
                    Assert.True(queue.Contains(i));
                }

                queue = BuildWrappedAroundQueue();

                for (i = 0; i < 4; i++)
                {
                    Assert.False(queue.Contains(i));
                }

                for (; i < 10; i++)
                {
                    Assert.True(queue.Contains(i));
                }

                Assert.False(queue.Contains(null));

                queue.Enqueue(null);

                Assert.True(queue.Contains(null));
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void ToArrayTest()
        {
            try
            {
                Object[] objArray = BuildNormalQueue().ToArray();

                int i = 4;
                foreach(Object o in objArray)
                {
                    Assert.True(i++ == (int)o);
                }

                objArray = BuildWrappedAroundQueue().ToArray();
                i = 4;
                foreach (Object o in objArray)
                {
                    Assert.True(i++ == (int)o);
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        private Queue BuildNormalQueue()
        {
            Queue queue = new Queue();

            for (int i = 2; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            queue.Dequeue();
            queue.Dequeue();

            return queue;
        }

        private Queue BuildWrappedAroundQueue()
        {
            Queue queue = new Queue();

            for (int i = 0; i < 8; i++)
            {
                queue.Enqueue(i);
            }

            for (int i = 0; i < 4; i++)
            {
                queue.Dequeue();
            }

            for (int i = 8; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            return queue;
        }
    }
}
