# IotaMamWraper

#### This is a simple interface to work around [Tangle.Net.Mam's](https://github.com/Felandil/tangle-.net) implementation over the native IOTA API.



---

### Dependencies.

AsyncBridge and Tangle.Net/Tangle.Net.Mam (which has some dependencies on its own).

---
#### Things to keep in mind.

* The simplicity of this API is paired, as usual, with a tendency to default parameters under the hood. 
If you need more configurability, there are some constructor overloads that allow a certain level of control (e.g, choosing the channel mode (public/private/restricted), the timeout and the like). Let your IDE guide you through the options.

* The seeds are randomly generated with Tangle.Net's abstraction, Seed.Random().
See: https://github.com/Felandil/tangle-.net/blob/develop/Tangle.Net/Tangle.Net/Entity/Seed.cs

* The connection will choose the first valid node in the given collection, if any.
If it doesn't receive any nodes, it'll throw an exception. However, if it _does_ receive at least one node, but is unable to connect to it/them, it won't throw any exceptions. A sanity check can be made through the CanWrite() method, prior to writting.

* The Connection won't handle exceptions coming from the raw IOTA API. For example, GetFirstMessage() may throw an Exception due to a findTransactions command failure.

* It's worth notting that, since we're working with MAM, we can only make zero-value transactions; i.e., it isn't possible to spend or receive tokens through the Tangle.




####            // *************** Usage notes. *************** \\

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
