using IOTAAPI.Lib;
using IOTAAPI.Test.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTAAPI.Test.AsynchronousAPITests
{
    [TestFixture]
    public class IotaAsyncConnectionPublicTest
    {
        [Test]
        public async Task TestPublicWriteMessages()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

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
        public async Task TestPublicWriteMessagesAndGetState()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);
            var State = await conn.WriteAndGetStateAsync("SomeMessage");
            Assert.IsNotNull(State);
        }
        [Test]
        public async Task TestPublicGetLastMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            await conn.WriteAsync("SomeMessage");

            var Message = await conn.GetLastMessageAsync();

            Assert.True(Message == "SomeMessage");
        }
        [Test]
        public async Task TestPublicGetFirstMessage()
        {
            IotaMamConnection conn = new IotaMamConnection(TestRepo.DevnetNode);

            Assert.True(conn.IsConnected);

            await conn.WriteAsync("SomeMessage");

            var Message = await conn.GetFirstMessageAsync();

            Assert.True(Message == "SomeMessage");
        }
    }
}
