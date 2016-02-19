namespace nsds.RedisDictionaryService.Test
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Void = nsds.RedisDictionaryService.Void;

    [TestFixture]
    public class AsyncRedisDictionaryServiceTest
    {
        private string connStr;
        [SetUp]
        public void TestSetUp()
        {
            this.connStr = System.Environment.GetEnvironmentVariable("REDIS_SERVER");
        }

        [Test]
        public void TestSetGetString()
        {
            var ds = new AsyncRedisDictionaryService(this.connStr);
            var futures = new List<AsyncFuture<Void>>();
            for (var i = 0; i < 10; i++)
            {
                futures.Add(ds.Add("Key" + i, "Value" + i));
            }

            var done = false;
            while (!done)
            {
                done = true;
                futures
                    .ForEach(
                        future => done &= future.Done);
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(ds.Get("Key" + i).Get(), "Value" + i);
            }
        }

        [TearDown]
        public void ClearDb()
        {
            var ds = new AsyncRedisDictionaryService(this.connStr, AllowAdmin: true);
            ds.Clear();
        }

        private TestClass CreateTestObject(string id, DateTime td, int tint, bool tbool)
        {
            return new TestClass { Id = id, TDate = td, Tint = tint, Tbool = tbool };
        }
    }
}