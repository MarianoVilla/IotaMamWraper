using IOTAAPI.Lib;
using IOTAAPI.Test.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangle.Net.Mam.Entity;

namespace IOTAAPI.Test.SynchronousAPITests
{
    [TestFixture]
    public class IotaConnectionRestrictedTest
    {
        [Test]
        public void TestRestrictedWriteMessages()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Restricted, TestRepo.DevnetNode);

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
        public void TestRestrictedWriteMessagesAndGetState()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Restricted, TestRepo.DevnetNode);
            var State = conn.WriteAndGetState("SomeMessage");
            Assert.IsNotNull(State);
        }
        [Test]
        public void TestRestrictedGetLastMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Restricted, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetLastMessage();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public void TestRestrictedGetFirstMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Restricted, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetFirstMessage();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public void TestRestrictedGetPublishedMessagesFromRootAndKey()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Restricted, TestRepo.DevnetNode);

            conn.Write("SomeMessage");

            var Messages = conn.GetPublishedMessages(conn.Root, conn.ChannelKey);

            Assert.True(Messages.Count == 1);
            Assert.True(Messages[0] == "SomeMessage");
        }
    }
}
