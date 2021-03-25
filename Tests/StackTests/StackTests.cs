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

                Assert.True(19 == stack.Count, "Stack.Count is incorrect");

                Assert.True(TestObjects.en.Equals(stack.Peek()));
                Assert.True(TestObjects.en.Equals(stack.Pop()));
                Assert.True(TestObjects.nul.Equals(stack.Peek()));
                Assert.True(TestObjects.nul.Equals(stack.Pop()));
                Assert.True(TestObjects.o.Equals(stack.Peek()));
                Assert.True(TestObjects.o.Equals(stack.Pop()));
                Assert.True(TestObjects.cl.Equals(stack.Peek()));
                Assert.True(TestObjects.cl.Equals(stack.Pop()));
                Assert.True(TestObjects.st.Equals(stack.Peek()));
                Assert.True(TestObjects.st.Equals(stack.Pop()));
                Assert.True(TestObjects.ts.Equals(stack.Peek()));
                Assert.True(TestObjects.ts.Equals(stack.Pop()));
                Assert.True(TestObjects.dt.Equals(stack.Peek()));
                Assert.True(TestObjects.dt.Equals(stack.Pop())); 
                Assert.True(TestObjects.str.Equals(stack.Peek()));
                Assert.True(TestObjects.str.Equals(stack.Pop()));
                Assert.True(TestObjects.c2.Equals(stack.Peek()));
                Assert.True(TestObjects.c2.Equals(stack.Pop()));
                Assert.True(TestObjects.f8.Equals(stack.Peek()));
                Assert.True(TestObjects.f8.Equals(stack.Pop()));
                Assert.True(TestObjects.f4.Equals(stack.Peek()));
                Assert.True(TestObjects.f4.Equals(stack.Pop()));
                Assert.True(TestObjects.s8.Equals(stack.Peek()));
                Assert.True(TestObjects.s8.Equals(stack.Pop()));
                Assert.True(TestObjects.u8.Equals(stack.Peek()));
                Assert.True(TestObjects.u8.Equals(stack.Pop()));
                Assert.True(TestObjects.s4.Equals(stack.Peek()));
                Assert.True(TestObjects.s4.Equals(stack.Pop()));
                Assert.True(TestObjects.u4.Equals(stack.Peek()));
                Assert.True(TestObjects.u4.Equals(stack.Pop()));
                Assert.True(TestObjects.s2.Equals(stack.Peek()));
                Assert.True(TestObjects.s2.Equals(stack.Pop()));
                Assert.True(TestObjects.u2.Equals(stack.Peek()));
                Assert.True(TestObjects.u2.Equals(stack.Pop()));
                Assert.True(TestObjects.s1.Equals(stack.Peek()));
                Assert.True(TestObjects.s1.Equals(stack.Pop()));
                Assert.True(TestObjects.u1.Equals(stack.Peek()));
                Assert.True(TestObjects.u1.Equals(stack.Pop()));

                Assert.True(0 == stack.Count, "Stack.Count is incorrect");
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

                Assert.True(8 == stack.Count, "Stack.Count is incorrect");

                for (int i = 7; i >= 4; i--)
                {
                    Assert.True(i.Equals(stack.Peek()));
                    Assert.True(i.Equals(stack.Pop()));
                }

                // in stack [ 3, 2, 1, 0 ]

                Assert.True(4 == stack.Count, "Stack.Count is incorrect");

                for (int i = 8; i < 12; i++)
                {
                    stack.Push(i);
                }

                // in stack [ 11, 10, 9, 8, 3, 2, 1, 0 ]

                Assert.True(8 == stack.Count, "Stack.Count is incorrect");

                for (int i = 11; i >= 10; i--)
                {
                    Assert.True(i.Equals(stack.Peek()));
                    Assert.True(i.Equals(stack.Pop()));
                }

                // in stack [ 9, 8, 3, 2, 1, 0 ]

                Assert.True(6 == stack.Count, "Stack.Count is incorrect");

                for (int i = 12; i < 16; i++)
                {
                    stack.Push(i);
                }

                // in stack [ 15, 14, 13, 12, 9, 8, 3, 2, 1, 0 ]

                Assert.True(10 == stack.Count, "Stack.Count is incorrect");

                for (int i = 15; i >= 12; i--)
                {
                    Assert.True(i.Equals(stack.Peek()));
                    Assert.True(i.Equals(stack.Pop()));
                }
                for (int i = 9; i >= 8; i--)
                {
                    Assert.True(i.Equals(stack.Peek()));
                    Assert.True(i.Equals(stack.Pop()));
                }
                for (int i = 3; i >= 0; i--)
                {
                    Assert.True(i.Equals(stack.Peek()));
                    Assert.True(i.Equals(stack.Pop()));
                }

                Assert.True(0 == stack.Count, "Stack.Count is incorrect");
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

                Assert.True(20 == stack.Count, "Stack.Count is incorrect");

                for (int i = 0; i < 20; i++)
                {
                    Assert.Null(stack.Pop());
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

                Assert.True(0 == stack.Count, "Stack.Count is incorrect");
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

                Assert.True(10 == stack.Count, "Stack.Count is incorrect");
                Assert.True(10 == clone.Count, "Stack.Count is incorrect");

                for (int i = 9; i >= 0; i--)
                {
                    Assert.True(i.Equals(stack.Pop()));
                }

                Assert.True(0 == stack.Count, "Stack.Count is incorrect");
                Assert.True(10 == clone.Count, "Stack.Count is incorrect");

                for (int i = 9; i >= 0; i--)
                {
                    Assert.True(i.Equals(clone.Pop()));
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
                    Assert.True(9 - i == intArray[i]);
                    Assert.True(9 - i == (int)objArray[i]);
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
                    Assert.True(j-- == i);
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
                    Assert.True(stack.Contains(i));
                }

                for (; i < 20; i++)
                {
                    Assert.False(stack.Contains(i));
                }

                Assert.False(stack.Contains(null));

                stack.Push(null);

                Assert.True(stack.Contains(null));
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
                    Assert.True(i-- == (int)o);
                }
            }
            catch (Exception e)
            {
                new Exception("Unexpected exception", e);
            }
        }
    }
}
