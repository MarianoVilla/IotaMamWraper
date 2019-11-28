# IotaMamWraper

#### This is a simple interface to work around [Tangle.Net.Mam's](https://github.com/Felandil/tangle-.net) implementation over the native IOTA API.



---

######            // *************** Usage notes. *************** \\

           IotaMamConnection conn;



            **//Instantiating.**
            //
            //The most basic constructor takes at least one node. 
            //If provided with more than one, **it'll prefeer the first valid (connectable) in the parameters list.**
            conn = new IotaMamConnection("https://nodes.devnet.iota.org:443");

            //In the next case the connection will use the first node, being the first valid node.
            conn = new IotaMamConnection("https://nodes.devnet.iota.org:443", "SomeInvalidaAddress");

            //Same here, it'll use the second one, since it's the first valid/usable node.
            conn = new IotaMamConnection("SomeInvalidaAddress", "https://nodes.devnet.iota.org:443");

            //There are also a few extra overloads for the constructor, so you can set the type of channel, the timeout and so.

            **//Writting.**
            //
            //Fire and forget:
            conn.Write("SomeMessage");
            conn.WriteAsync("SomeMessage");
            
            //In the fashion of 
            //[HyperledgerFabric-IOTA-Connector](https://github.com/iotaledger/HyperledgerFabric-IOTA-Connector),
            //the next two methods write a message and return a JSON representation of the channel:
            conn.WriteAndGetState("SomeMessage");
            conn.WriteAndGetStateAsync("SomeMessage");

            **//Getting published messages (as strings)**
            //
            conn.GetPublishedMessages();
            conn.GetPublishedMessagesAsync();
            
            //The next methods are a shorcut, but they retrieve the messages each time they're called,
            //so LINQing the collection
            //yourself would be a better approach (if it hasn't changed).
            conn.GetFirstMessage();
            conn.GetFirstMessageAsync();
            conn.GetLastMessage();
            conn.GetLastMessageAsync();

            **//State.**
            //
            //Returns true if the connection got at least one usable node.
            var IsConnected = conn.IsConnected;

            //Returns the URI of the node that's connected.
            var ConnectedNode = conn.ConnectedNode;
            
            //Returns the connection seed.
            var ConnectionSeed = conn.ConnectionSeed

            //Returns the channel key.
            var ChannelKey = conn.ChannelKey

            //Local/Remote/PowSrv. 
            //See: https://medium.com/bytes-io/iota-proof-of-work-remote-vs-local-explained-1cbd89392a79
            var PoW = conn.PoW;

            //Gets the connection timeout. By the time of this writting, it has to be set at construction time.
            var Timeout = conn.Timeout;

            //If you wish to cache the invalid nodes to prevent reattempts and improve efficiency,
            //this list may save you some time.
            var InvalidNodes = conn.InvalidNodesReceived;

            //Public/Private/Restricted. See: https://blog.iota.org/introducing-masked-authenticated-messaging-e55c1822d50e
            var ChannelMode = conn.ChannelMode;
