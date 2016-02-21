using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using nsds.Exceptions;

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
        public async Task TestAddAsync()
        {
            var ds = new RedisDictionaryService(this.connStr);
            var t = ds.AddAsync("TestKey", "Hello");
            await t;
            Assert.AreEqual(1, ds.Count);
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
        public async Task TestRemoveKeyAsync()
        {
            var ds = new RedisDictionaryService(this.connStr);
            var removedKey = "TestRemoveKey";
            ds[removedKey] = "Will be removed";
            try
            {
                var task = ds.RemoveAsync(removedKey);
                await task;
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
            var value = (string) ds["TestStringKey"];
            Assert.AreEqual(value, "Hello");
        }

        [Test]
        public async Task TestSetGetStringAsync()
        {
            var ds = new RedisDictionaryService(this.connStr);
            await ds.AddAsync("TestStringKey", "Hello");
            var value = await ds.GetAsync("TestStringKey");
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
            var retObject1 = (TestClass) ds["TO1"];
            var retObject2 = (TestClass) ds["TO2"];
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
            foreach (var kp in ds)
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
            for (int i = 0; i < numOfObj; i++)
            {
                var key = "key" + i;
                ds[key] = new TestClass {Id = key, TDate = DateTime.Now, Tbool = false, Tint = i};
            }
            Assert.AreEqual(ds.Keys.Count, numOfObj);
        }

        [Test]
        public void TestExpireTime()
        {
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            var ds = new RedisDictionaryService(this.connStr);
            ds.Add("TO1", to1, TimeSpan.FromSeconds(3));
            ds.Add("TO2", to2);
            Thread.Sleep(5*1000);
            Assert.AreEqual(1, ds.Keys.Count);
            var retObject2 = (TestClass) ds["TO2"];
            Assert.AreEqual(retObject2.Id, to2.Id);
        }

        [Test]
        public void TestExpireTime2()
        {
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            var ds = new RedisDictionaryService(this.connStr);
            ds.Add("TO1", to1, TimeSpan.FromSeconds(5));
            ds.Add("TO2", to2);
            Thread.Sleep(7*1000);
            Assert.AreEqual(1, ds.Keys.Count);
            ds.Add("TO1", to1, TimeSpan.FromSeconds(1));
            Thread.Sleep(2*1000);
            Assert.AreEqual(1, ds.Keys.Count);
            var retObject2 = (TestClass) ds["TO2"];
            Assert.AreEqual(retObject2.Id, to2.Id);
        }

        [Test]
        public void TestRandomAccessWithExpire()
        {
            var ds = new RedisDictionaryService(this.connStr);
            var numOfObj = 20000;
            Random rnd = new Random();
            for (int i = 0; i < numOfObj; i++)
            {
                var key = "key" + i;
                var obj = new TestClass {Id = key, TDate = DateTime.Now, Tbool = false, Tint = i};
                ds.Add(key, obj, TimeSpan.FromSeconds(rnd.Next(1, 5)));
            }

            for (int i = 0; i < numOfObj; i++)
            {
                try
                {
                    var obj = ds["key" + rnd.Next(0, numOfObj)];
                }
                catch (Exception)
                {
                }
            }
        }

        [Test]
        public void TestDbNumber()
        {
            try
            {
                var ds = new RedisDictionaryService(this.connStr, databaseNumber: 0, timeDatabaseNumber: 0);
                Assert.True(false);
            }
            catch (NsdsException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test]
        public async Task TestClearDbAsync()
        {
            
            var ds = new RedisDictionaryService(this.connStr, AllowAdmin: true);
            var to1 = CreateTestObject("001", DateTime.Now, 10, false);
            var to2 = CreateTestObject("002", DateTime.Now.AddDays(1), 12, true);
            ds["TO1"] = to1;
            ds["TO2"] = to2;
            await ds.ClearAsync();
            Assert.AreEqual(0, ds.Count);
        }

        [TearDown]
        public void ClearDb()
        {
            var ds = new RedisDictionaryService(this.connStr, AllowAdmin: true);
            ds.Clear();
        }

        private TestClass CreateTestObject(string id, DateTime td, int tint, bool tbool)
        {
            return new TestClass {Id = id, TDate = td, Tint = tint, Tbool = tbool};
        }
    }
}