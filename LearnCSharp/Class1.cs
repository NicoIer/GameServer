using System;
using System.Collections.Generic;

namespace LearnCSharp
{
    public static class Funtions
    {
        public struct TestListStruct
        {
            public List<int> number;
        }

        public static void TestNewList()
        {
            TestListStruct s1 = new TestListStruct()
            {
                number = new List<int>()
                {
                    1, 2, 3, 4, 5
                }
            };
            TestListStruct s2 = new TestListStruct()
            {
                number = { 6, 7, 8, 9, 10 }
            };
        }

        enum ItemEnum
        {
            None = 0,
            Coin = 1
        }

        public static void TestEnum()
        {
            Console.WriteLine((ItemEnum)(-1) == ItemEnum.Coin);
            Console.WriteLine((ItemEnum)(-1));
            Console.WriteLine((ItemEnum)0);

            Console.WriteLine((ItemEnum)1);

            Console.WriteLine((ItemEnum)2);
        }
    }
}