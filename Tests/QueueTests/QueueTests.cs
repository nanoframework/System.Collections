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
        private static int _queueElementCount = 0;

        [TestMethod]
        public void EnqueueDequeuePeekTest1()
        {
            Queue queue = new();

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

            Assert.True(19 == queue.Count, $"Queue.Count is incorrect. Expecting 18 got {queue.Count}");

            Assert.True(TestObjects.u1.Equals(queue.Peek()), "u2 Peek failed");
            Assert.True(TestObjects.u1.Equals(queue.Dequeue()), "u2 Dequeue failed");
            Assert.True(TestObjects.s1.Equals(queue.Peek()), "s1 Peek failed");
            Assert.True(TestObjects.s1.Equals(queue.Dequeue()), "s1 Dequeue failed");
            Assert.True(TestObjects.u2.Equals(queue.Peek()), "u2 Peek failed");
            Assert.True(TestObjects.u2.Equals(queue.Dequeue()), "u2 Dequeue failed");
            Assert.True(TestObjects.s2.Equals(queue.Peek()), "s2 Peek failed");
            Assert.True(TestObjects.s2.Equals(queue.Dequeue()), "s2 Dequeue failed");
            Assert.True(TestObjects.u4.Equals(queue.Peek()), "u4 Peek failed");
            Assert.True(TestObjects.u4.Equals(queue.Dequeue()), "u4 Dequeue failed");
            Assert.True(TestObjects.s4.Equals(queue.Peek()), "s4 Peek failed");
            Assert.True(TestObjects.s4.Equals(queue.Dequeue()), "s4 Dequeue failed");
            Assert.True(TestObjects.u8.Equals(queue.Peek()), "u8 Peek failed");
            Assert.True(TestObjects.u8.Equals(queue.Dequeue()), "u8 Dequeue failed");
            Assert.True(TestObjects.s8.Equals(queue.Peek()), "s8 Peek failed");
            Assert.True(TestObjects.s8.Equals(queue.Dequeue()), "s8 Dequeue failed");
            Assert.True(TestObjects.f4.Equals(queue.Peek()), "f4 Peek failed");
            Assert.True(TestObjects.f4.Equals(queue.Dequeue()), "f4 Dequeue failed");
            Assert.True(TestObjects.f8.Equals(queue.Peek()), "f8 Peek failed");
            Assert.True(TestObjects.f8.Equals(queue.Dequeue()), "f8 Dequeue failed");
            Assert.True(TestObjects.c2.Equals(queue.Peek()), "c2 Peek failed");
            Assert.True(TestObjects.c2.Equals(queue.Dequeue()), "c2 Dequeue failed");
            Assert.True(TestObjects.str.Equals(queue.Peek()), "str Peek failed");
            Assert.True(TestObjects.str.Equals(queue.Dequeue()), "str Dequeue failed");
            Assert.True(TestObjects.dt.Equals(queue.Peek()), "dt Peek failed");
            Assert.True(TestObjects.dt.Equals(queue.Dequeue()), "dt Dequeue failed");
            Assert.True(TestObjects.ts.Equals(queue.Peek()), "ts Peek failed");
            Assert.True(TestObjects.ts.Equals(queue.Dequeue()), "ts Dequeue failed");
            Assert.True(queue.Peek().Equals(TestObjects.st), "st Peek failed");
            Assert.True(queue.Dequeue().Equals(TestObjects.st), "st Dequeue failed");
            Assert.True(TestObjects.cl.Equals(queue.Peek()), "cl Peek failed");
            Assert.True(TestObjects.cl.Equals(queue.Dequeue()), "cl Dequeue failed");
            Assert.True(TestObjects.o.Equals(queue.Peek()), "o Peek failed");
            Assert.True(TestObjects.o.Equals(queue.Dequeue()), "o Dequeue failed");
            Assert.True(queue.Peek() is null, "nul Peek failed");
            Assert.True(queue.Dequeue() is null, "nul Dequeue failed");
            Assert.True(TestObjects.en.Equals(queue.Peek()), "en Peek failed");
            Assert.True(TestObjects.en.Equals(queue.Dequeue()), "en Dequeue failed");

            Assert.True(0 == queue.Count, $"Queue.Count is incorrect. Expecting 0 got {queue.Count}");
        }

        [TestMethod]
        public void EnqueueDequeuePeekTest2()
        {
            Queue queue = new();

            for (int i = 0; i < 8; i++)
            {
                queue.Enqueue(i);
            }

            Assert.True(8 == queue.Count, "Queue.Count is incorrect");

            for (int i = 0; i < 4; i++)
            {
                Assert.True(i.Equals(queue.Peek()), " Peek failed");
                Assert.True(i.Equals(queue.Dequeue()), " Dequeue failed");
            }

            Assert.True(4 == queue.Count, "Queue.Count is incorrect");

            for (int i = 8; i < 12; i++)
            {
                queue.Enqueue(i);
            }

            Assert.True(8 == queue.Count, "Queue.Count is incorrect");

            for (int i = 4; i < 6; i++)
            {
                Assert.True(i.Equals(queue.Peek()), " Peek failed");
                Assert.True(i.Equals(queue.Dequeue()), " Dequeue failed");
            }

            Assert.True(6 == queue.Count, "Queue.Count is incorrect");

            for (int i = 12; i < 16; i++)
            {
                queue.Enqueue(i);
            }

            Assert.True(10 == queue.Count, "Queue.Count is incorrect");

            for (int i = 6; i < 16; i++)
            {
                Assert.True(i.Equals(queue.Peek()), " Peek failed");
                Assert.True(i.Equals(queue.Dequeue()), " Dequeue failed");
            }

            Assert.True(0 == queue.Count, "Queue.Count is incorrect");
        }

        [TestMethod]
        public void EnqueueDequeueNullTest()
        {
            Queue queue = new();

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

        [TestMethod]
        public void QueueClearTest()
        {
            OutputHelper.WriteLine("Starting ClearTest");

            Queue queue = BuildNormalQueue();

            queue.Clear();

            Assert.True(0 == queue.Count, "Queue.Count is incorrect");

            queue = BuildWrappedAroundQueue();

            queue.Clear();

            Assert.True(0 == queue.Count, "Queue.Count is incorrect");
        }

        [TestMethod]
        public void QueueCloneTest()
        {
            OutputHelper.WriteLine("Testing with BuildNormalQueue");

            Queue queue = BuildNormalQueue();
            Queue clone = (Queue)queue.Clone();

            Assert.True(_queueElementCount == queue.Count, "Queue.Count is incorrect");
            Assert.True(_queueElementCount == clone.Count, "Queue.Count is incorrect");

            for (int i = 2; i < 10; i++)
            {
                var element = queue.Dequeue();
                Assert.True(i.Equals(element), $" Dequeue queue failed. Got {element}, was expecting {i}");
            }

            Assert.True(0 == queue.Count, $"queue.Count is incorrect. Is {queue.Count} and expecting 0.");
            Assert.True(_queueElementCount == clone.Count, $"clone.Count is incorrect. Is {clone.Count} and expecting {_queueElementCount}.");

            for (int i = 2; i < 10; i++)
            {
                var element = clone.Dequeue();
                Assert.True(i.Equals(element), $" Dequeue clone failed. Got {element}, was expecting {i}");
            }

            OutputHelper.WriteLine("Testing with BuildWrappedAroundQueue");

            queue = BuildWrappedAroundQueue();
            clone = (Queue)queue.Clone();

            Assert.True(_queueElementCount == queue.Count, $"queue.Count is incorrect. Is {queue.Count} and expecting {_queueElementCount}.");
            Assert.True(_queueElementCount == clone.Count, $"clone.Count is incorrect. Is {clone.Count} and expecting {_queueElementCount}.");

            for (int i = 4; i < 10; i++)
            {
                var element = queue.Dequeue();
                Assert.True(i.Equals(element), $" Dequeue queue failed. Got {element}, was expecting {i}");
            }

            Assert.True(0 == queue.Count, $"queue.Count is incorrect. Is {queue.Count} and expecting 0.");
            Assert.True(_queueElementCount == clone.Count, $"clone.Count is incorrect. Is {clone.Count} and expecting {_queueElementCount}.");

            for (int i = 4; i < 10; i++)
            {
                var element = clone.Dequeue();
                Assert.True(i.Equals(element), $" Dequeue clone failed. Got {element}, was expecting {i}");
            }
        }

        [TestMethod]
        public void QueueCopyToTest()
        {
            OutputHelper.WriteLine("Testing with BuildNormalQueue");

            Queue queue = BuildNormalQueue();
            int[] intArray = new int[10];
            Object[] objArray = new Object[10];

            OutputHelper.WriteLine("Copying to intArray");
            queue.CopyTo(intArray, 2);

            OutputHelper.WriteLine("Copying to objArray");
            queue.CopyTo(objArray, 2);

            for (int i = 2; i < 10; i++)
            {
                Assert.True(i.Equals(intArray[i]));
                Assert.True(i.Equals((int)objArray[i]));
            }

            OutputHelper.WriteLine("Testing with BuildWrappedAroundQueue");

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

        [TestMethod]
        public void QueueEnumeratorTest()
        {
            Queue queue = BuildNormalQueue();

            int j = 2;

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

        [TestMethod]
        public void QueueContainsTest()
        {
            Queue queue = BuildNormalQueue();
            int i;

            for (i = 2; i < 4; i++)
            {
                Assert.True(queue.Contains(i), $"normal queue doesn't contain {i} and it was supposed to");
            }

            for (; i < 10; i++)
            {
                Assert.True(queue.Contains(i), $"normal queue doesn't contain {i} and it was supposed to");
            }

            queue = BuildWrappedAroundQueue();

            for (i = 0; i < 4; i++)
            {
                Assert.False(queue.Contains(i), $"wrapped queue doesn't contain {i} and it was supposed to");
            }

            for (; i < 10; i++)
            {
                Assert.True(queue.Contains(i), $"wrapped queue doesn't contain {i} and it was supposed to");
            }

            Assert.False(queue.Contains(null), "queue wasn't supposed to contain null");

            queue.Enqueue(null);

            Assert.True(queue.Contains(null), "queue was supposed to contain null");
        }

        [TestMethod]
        public void QueueToArrayTest()
        {
            Object[] objArray = BuildNormalQueue().ToArray();

            int i = 2;
            foreach (Object o in objArray)
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

        private Queue BuildNormalQueue()
        {
            Queue queue = new();

            for (int i = 0; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            // update counter            
            _queueElementCount = 10;

            Assert.True(queue.Count == _queueElementCount, "Count of NormalQueue elements failed");

            queue.Dequeue();
            _queueElementCount--;

            queue.Dequeue();
            _queueElementCount--;

            Assert.True(queue.Count == _queueElementCount, "Count of NormalQueue elements failed after dequeuing");

            return queue;
        }

        private Queue BuildWrappedAroundQueue()
        {
            Queue queue = new();

            for (int i = 0; i < 8; i++)
            {
                queue.Enqueue(i);
            }

            _queueElementCount = 8;

            for (int i = 0; i < 4; i++)
            {
                queue.Dequeue();
            }

            _queueElementCount -= 4;

            for (int i = 8; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            _queueElementCount += 2;

            return queue;
        }
    }
}
