using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aim.UnitTests
{
    public class SQLTest
    {
        public static void Test() 
        {
            TestClass tc = new TestClass();
            tc.testString = "I AM string!";
            tc.testInt = 114514;

            var entity = StonePlanner.AccessEntity.GetAccessEntityInstance(@"D:\Projects\Aim\StonePlanner\bin\Debug\data.mdb", "114514");
            string ver = entity.AddElement(tc,"tb_test");
            Console.WriteLine(ver);
        }

        public class TestClass
        {
            public string testString { get; set; }
            public int testInt { get; set; }
        }
    }
}
