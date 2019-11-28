using IOTAAPI.Lib;
using IOTAAPI.Test.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IOTAAPI.Test.SynchronousAPITests
{
    [TestFixture]
    public class IotaConnectionGeneralTest
    {
        [Test]
        public void TestCreateWithSingleNode()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);
            Assert.AreEqual(TestRepo.DevnetNode, conn.ConnectedNode);
        }
        [Test]
        public void TestCreateWithMultipleNodes_ShouldPickDev()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode, "SomeInvalidNode");
            Assert.True(conn.IsConnected);
            Assert.AreEqual(TestRepo.DevnetNode, conn.ConnectedNode);

            conn = new IotaMamConnection("SomeInvalidNode", TestRepo.DevnetNode);
            Assert.True(conn.IsConnected);
            Assert.AreEqual(TestRepo.DevnetNode, conn.ConnectedNode);

            Assert.True(conn.InvalidNodesReceived.Count == 1);
            Assert.True(conn.InvalidNodesReceived.Contains("SomeInvalidNode"));
        }
        [Test]
        public void TestCreateWithoutNodes_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new IotaMamConnection());
            Assert.Throws<ArgumentException>(() => new IotaMamConnection(null));
            Assert.Throws<ArgumentException>(() => new IotaMamConnection(""));
        }
        [Test]
        public void TestCantWriteWithoutNode()
        {
            IotaMamConnection conn = new IotaMamConnection("SomeInvalidNode");
            Assert.False(conn.IsConnected);
        }
        [Test]
        public void TestShortTimeout_ShouldThrow()
        {
            IotaMamConnection conn = new IotaMamConnection(1, TestRepo.DevnetNode);
            var Thrown = Assert.Throws<AggregateException>(() => conn.Write("SomeMessage"));
            Assert.True(Thrown.InnerException is WebException);
        }
    }
}
