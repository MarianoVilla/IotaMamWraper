using IOTAAPI.Lib;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTAAPI.ConsoleProj
{
    class Program
    {
        static void Main(string[] args)
        {

            IotaMamConnection conn;

            // *************** Notes. *************** \\

            //Instantiating.
            //
            conn = new IotaMamConnection("https://nodes.devnet.iota.org:443");

            //Will use the first node, being the first valid node.
            conn = new IotaMamConnection("https://nodes.devnet.iota.org:443", "SomeInvalidaAddress");

            //Will use the second one, being the first valid node.
            conn = new IotaMamConnection("SomeInvalidaAddress", "https://nodes.devnet.iota.org:443");

            //There are a few extra overloads for the constructor, so you can set the type of channel or the timeout.

            //Writting.
            //
            conn.Write("SomeMessage");
            conn.WriteAsync("SomeMessage");
            conn.WriteAndGetState("SomeMessage");
            conn.WriteAndGetStateAsync("SomeMessage");

            //Getting published messages.
            //
            conn.GetPublishedMessages();
            conn.GetPublishedMessagesAsync();
            conn.GetFirstMessage();
            conn.GetFirstMessageAsync();
            conn.GetLastMessage();
            conn.GetLastMessageAsync();

            //State.
            //Returns true if the connection got at least one usable node.
            var IsConnected = conn.IsConnected;

            //Returns the URI of the node that's connected.
            var ConnectedNode = conn.ConnectedNode;

            //Local/Remote/PowSrv. See: https://medium.com/bytes-io/iota-proof-of-work-remote-vs-local-explained-1cbd89392a79
            var PoW = conn.PoW;

            //Gets the connection timeout. It has to be set at construction time.
            var Timeout = conn.Timeout;

            //If you wish to cache the invalid nodes to prevent reattempts and improve efficiency, this list may save you some time.
            var InvalidNodes = conn.InvalidNodesReceived;

            //Public/Private/Restricted. See: https://blog.iota.org/introducing-masked-authenticated-messaging-e55c1822d50e
            var ChannelMode = conn.ChannelMode;


            Console.ReadKey();
        }

    }
}
