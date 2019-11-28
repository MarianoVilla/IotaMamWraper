using IOTAAPI.Lib;
using IOTAAPI.Test.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangle.Net.Mam.Entity;

namespace IOTAAPI.Test.AsynchronousAPITests
{
    [TestFixture]
    public class IotaAsyncConnectionPrivateTest
    {
        [Test]
        public async Task TestPrivateWriteMessages()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            await conn.WriteAsync("SomeMessage");

            var Messages = await conn.GetPublishedMessagesAsync();

            Assert.True(Messages.Count == 1);
            Assert.True(Messages[0] == "SomeMessage");

            await conn.WriteAsync("SomeOtherMessage");

            Messages = await conn.GetPublishedMessagesAsync();

            Assert.True(Messages.Count == 2);
            Assert.True(Messages[1] == "SomeOtherMessage");

        }
        [Test]
        public async Task TestPrivateWriteMessagesAndGetState()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);
            var State = await conn.WriteAndGetStateAsync("SomeMessage");
            Assert.IsNotNull(State);
        }
        [Test]
        public async Task TestPrivateGetLastMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            await conn.WriteAsync("SomeMessage");

            var Message = await conn.GetLastMessageAsync();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public async Task TestPrivateGetFirstMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(Mode.Private, TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            await conn.WriteAsync("SomeMessage");

            var Message = await conn.GetFirstMessageAsync();

            Assert.True(Message == "SomeMessage");
        }
    }
}
