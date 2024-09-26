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
    public class StackTests
    {
        [TestMethod]
        public void PushPopPeekTest1()
        {
            try
            {
                Stack stack = new Stack();

                stack.Push(TestObjects.u1);
                stack.Push(TestObjects.s1);
                stack.Push(TestObjects.u2);
                stack.Push(TestObjects.s2);
                stack.Push(TestObjects.u4);
                stack.Push(TestObjects.s4);
                stack.Push(TestObjects.u8);
                stack.Push(TestObjects.s8);
                stack.Push(TestObjects.f4);
                stack.Push(TestObjects.f8);
                stack.Push(TestObjects.c2);
                stack.Push(TestObjects.str);
                stack.Push(TestObjects.dt);
                stack.Push(TestObjects.ts);
                stack.Push(TestObjects.st);
                stack.Push(TestObjects.cl);
                stack.Push(TestObjects.o);
                stack.Push(TestObjects.nul);
                stack.Push(TestObjects.en);

                Assert.IsTrue(19 == stack.Count, "Stack.Count is incorrect");

                Assert.IsTrue(TestObjects.en.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.en.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.nul.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.nul.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.o.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.o.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.cl.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.cl.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.st.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.st.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.ts.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.ts.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.dt.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.dt.Equals(stack.Pop())); 
                Assert.IsTrue(TestObjects.str.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.str.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.c2.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.c2.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.f8.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.f8.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.f4.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.f4.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.s8.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.s8.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.u8.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.u8.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.s4.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.s4.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.u4.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.u4.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.s2.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.s2.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.u2.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.u2.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.s1.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.s1.Equals(stack.Pop()));
                Assert.IsTrue(TestObjects.u1.Equals(stack.Peek()), " Peek failed");
                Assert.IsTrue(TestObjects.u1.Equals(stack.Pop()));

                Assert.IsTrue(0 == stack.Count, "Stack.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void PushPopPeekTest2()
        {
            try
            {
                Stack stack = new Stack();

                for (int i = 0; i < 8; i++)
                {
                    stack.Push(i);
                }

                // in stack [ 7, 6, 5, 4, 3, 2, 1, 0 ]

                Assert.IsTrue(8 == stack.Count, "Stack.Count is incorrect");

                for (int i = 7; i >= 4; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Peek()), " Peek failed");
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }

                // in stack [ 3, 2, 1, 0 ]

                Assert.IsTrue(4 == stack.Count, "Stack.Count is incorrect");

                for (int i = 8; i < 12; i++)
                {
                    stack.Push(i);
                }

                // in stack [ 11, 10, 9, 8, 3, 2, 1, 0 ]

                Assert.IsTrue(8 == stack.Count, "Stack.Count is incorrect");

                for (int i = 11; i >= 10; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Peek()), " Peek failed");
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }

                // in stack [ 9, 8, 3, 2, 1, 0 ]

                Assert.IsTrue(6 == stack.Count, "Stack.Count is incorrect");

                for (int i = 12; i < 16; i++)
                {
                    stack.Push(i);
                }

                // in stack [ 15, 14, 13, 12, 9, 8, 3, 2, 1, 0 ]

                Assert.IsTrue(10 == stack.Count, "Stack.Count is incorrect");

                for (int i = 15; i >= 12; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Peek()), " Peek failed");
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }
                for (int i = 9; i >= 8; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Peek()), " Peek failed");
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }
                for (int i = 3; i >= 0; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Peek()), " Peek failed");
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }

                Assert.IsTrue(0 == stack.Count, "Stack.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        [TestMethod]
        public void PushPopNullTest()
        {
            try
            {
                Stack stack = new Stack();

                for (int i = 0; i < 20; i++)
                {
                    stack.Push(null);
                }

                Assert.IsTrue(20 == stack.Count, "Stack.Count is incorrect");

                for (int i = 0; i < 20; i++)
                {
                    Assert.IsNull(stack.Pop());
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
                Stack stack = BuildStack();

                stack.Clear();

                Assert.IsTrue(0 == stack.Count, "Stack.Count is incorrect");
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }

        private Stack BuildStack()
        {
            Stack stack = new Stack();

            for (int i = 0; i < 10; i++)
            {
                stack.Push(i);
            }

            return stack;
        }

        [TestMethod]
        public void CloneTest()
        {
            try
            {
                Stack stack = BuildStack();
                Stack clone = (Stack)stack.Clone();

                Assert.IsTrue(10 == stack.Count, "Stack.Count is incorrect");
                Assert.IsTrue(10 == clone.Count, "Stack.Count is incorrect");

                for (int i = 9; i >= 0; i--)
                {
                    Assert.IsTrue(i.Equals(stack.Pop()));
                }

                Assert.IsTrue(0 == stack.Count, "Stack.Count is incorrect");
                Assert.IsTrue(10 == clone.Count, "Stack.Count is incorrect");

                for (int i = 9; i >= 0; i--)
                {
                    Assert.IsTrue(i.Equals(clone.Pop()));
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
                Stack stack = BuildStack();
                int[] intArray = new int[10];
                Object[] objArray = new Object[10];

                stack.CopyTo(intArray, 0);
                stack.CopyTo(objArray, 0);

                for (int i = 0; i < 10; i++)
                {
                    Assert.IsTrue(9 - i == intArray[i]);
                    Assert.IsTrue(9 - i == (int)objArray[i]);
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
                Stack stack = BuildStack();

                int j = 9;

                foreach (int i in stack)
                {
                    Assert.IsTrue(j-- == i);
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
                Stack stack = BuildStack();
                int i;

                for (i = 0; i < 10; i++)
                {
                    Assert.IsTrue(stack.Contains(i));
                }

                for (; i < 20; i++)
                {
                    Assert.IsFalse(stack.Contains(i));
                }

                Assert.IsFalse(stack.Contains(null));

                stack.Push(null);

                Assert.IsTrue(stack.Contains(null));
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
                Object[] objArray = BuildStack().ToArray();

                int i = 9;
                foreach (Object o in objArray)
                {
                    Assert.IsTrue(i-- == (int)o);
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }
    }
}
