using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsds.RedisDictionaryService.Test
{

    [TestFixture]
    public class RedisDictionaryServiceTest
    {
        private string connStr;

        [SetUp]
        public void TestSetUp()
        {
            this.connStr = System.Environment.GetEnvironmentVariable("REDIS_SERVER");
        }

        [Test]
        public void TestConnect()
        {
            var ds = new RedisDictionaryService(this.connStr);
            ds["TestKey"] = "Hello";
        }


        [Test]
        public void TestSetGetString()
        {
            var ds = new RedisDictionaryService(this.connStr);
            ds["TestStringKey"] = "Hello";
            var value = (string)ds["TestStringKey"];
            Assert.AreEqual(value, "Hello");
        }

        [TearDown]
        public void ClearDb()
        {
        }
    }
}
