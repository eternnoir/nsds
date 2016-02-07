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
        public void TestRemoveKey()
        {
            var ds = new RedisDictionaryService(this.connStr);
            var removedKey = "TestRemoveKey";
            ds[removedKey] = "Will be removed";
            try
            {
                ds.Remove(removedKey);
                var t = ds[removedKey];
                Assert.Fail("Key must not found.");
            }
            catch (KeyNotFoundException knfe)
            {

            }
            catch (Exception ex)
            {
                Assert.Fail("Must throw KeyNotFoundException.");
            }
        }

        [Test]
        public void TestSetGetString()
        {
            var ds = new RedisDictionaryService(this.connStr);
            ds["TestStringKey"] = "Hello";
            var value = (string)ds["TestStringKey"];
            Assert.AreEqual(value, "Hello");
        }

        [Test]
        public void TestComplexClass()
        {
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            var ds = new RedisDictionaryService(this.connStr);
            ds["TO1"] = to1;
            ds["TO2"] = to2;
            var retObject1 = (TestClass)ds["TO1"];
            var retObject2 = (TestClass)ds["TO2"];
            Assert.AreEqual(to1.Id, retObject1.Id);
            Assert.AreEqual(to1.TDate, retObject1.TDate);
            Assert.AreEqual(to1.Tint, retObject1.Tint);
            Assert.AreEqual(to1.Tbool, retObject1.Tbool);
            Assert.AreEqual(to2.Id, retObject2.Id);
            Assert.AreEqual(to2.TDate, retObject2.TDate);
            Assert.AreEqual(to2.Tint, retObject2.Tint);
            Assert.AreEqual(to2.Tbool, retObject2.Tbool);
        }

        [Test]
        public void TestEnumerable()
        {
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            var ds = new RedisDictionaryService(this.connStr);
            ds["TO1"] = to1;
            ds["TO2"] = to2;
            foreach(var kp in ds)
            {
                Assert.Pass(kp.ToString());
            }
        }

        [Test]
        public void TestGetKeys()
        {
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            var ds = new RedisDictionaryService(this.connStr);
            ds["TO1"] = to1;
            ds["TO2"] = to2;
            Assert.AreEqual(ds.Keys.Count, 2);
 
        }

        [Test]
        public void TestLotsInput()
        {
            var ds = new RedisDictionaryService(this.connStr);
            var numOfObj = 20000;
            for(int i = 0; i< numOfObj; i++)
            {
                var key = "key" + i;
                ds[key] = new TestClass { Id = key, TDate = DateTime.Now, Tbool = false, Tint = i };
            }
            Assert.AreEqual(ds.Keys.Count, numOfObj);
        }

        [TearDown]
        public void ClearDb()
        {
            var ds = new RedisDictionaryService(this.connStr, AllowAdmin: true);
            ds.Clear();
        }

        private TestClass CreateTestObject(string id, DateTime td, int tint, bool tbool)
        {
            return new TestClass { Id = id, TDate = td, Tint = tint, Tbool = tbool };
        }
    }
}
