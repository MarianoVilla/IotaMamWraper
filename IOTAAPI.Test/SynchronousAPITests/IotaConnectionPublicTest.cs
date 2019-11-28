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
    public class IotaConnectionPublicTest
    {
        [Test]
        public void TestPublicWriteMessages()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Messages = conn.GetPublishedMessages();

            Assert.True(Messages.Count == 1);
            Assert.True(Messages[0] == "SomeMessage");

            conn.Write("SomeOtherMessage");

            Messages = conn.GetPublishedMessages();

            Assert.True(Messages.Count == 2);
            Assert.True(Messages[1] == "SomeOtherMessage");

        }
        [Test]
        public void TestPublicWriteMessagesAndGetState()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);
            var State = conn.WriteAndGetState("SomeMessage");
            Assert.IsNotNull(State);
        }
        [Test]
        public void TestPublicGetLastMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetLastMessage();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public void TestPublicGetFirstMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetFirstMessage();

            Assert.True(Message == "SomeMessage");
        }

    }
}
