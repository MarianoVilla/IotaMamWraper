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
    public class IotaConnectionPrivateTest
    {
        [Test]
        public void TestPrivateWriteMessages()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

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
        public void TestPrivateWriteMessagesAndGetState()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);
            var State = conn.WriteAndGetState("SomeMessage");
            Assert.IsNotNull(State);
        }
        [Test]
        public void TestPrivateGetLastMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetLastMessage();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public void TestPrivateGetFirstMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            conn.Write("SomeMessage");

            var Message = conn.GetFirstMessage();

            Assert.True(Message == "SomeMessage");
        }
    }
}
