using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangle.Net.Repository;
using Tangle.Net.Repository.Factory;
using Tangle.Net.Mam;
using Tangle.Net.Mam.Services;
using Tangle.Net.Mam.Entity;
using Tangle.Net.Cryptography;
using Tangle.Net.Mam.Merkle;
using Tangle.Net.Entity;
using Tangle.Net.ProofOfWork;
using Newtonsoft.Json;
using Tangle.Net.Repository.DataTransfer;
using AsyncBridge;

namespace IOTAAPI.Lib
{

    public class IotaMamConnection
    {
        /// <summary>
        /// Most of the fields are inner state used to work around Tangle.Net.
        /// </summary>
        #region Fields.
        internal MamChannel Channel { get; private set; }
        internal MamChannelSubscription Subscription { get; private set; }
        internal MamChannelSubscriptionFactory SubscriptionFactory { get; private set; }
        internal MaskedAuthenticatedMessage FirstMessage { get; private set; }
        internal MamChannelFactory ChannelFactory { get; private set; }
        private IIotaRepository Factory;
        #endregion


        /// <summary>
        /// The public state is set at creation time.
        /// </summary>
        #region Props
        //The Channel mode: Public/Private/Restricted.
        public Mode ChannelMode { get; set; } = Mode.Public;

        public int Timeout { get; private set; } = int.MaxValue;

        //The type of Proof of Work: Local/Remote/PowSrv. See: https://medium.com/bytes-io/iota-proof-of-work-remote-vs-local-explained-1cbd89392a79
        public PoWType PoW { get; private set; } = PoWType.Remote;

        //Returns true if the connection got at least one usable node.
        public bool IsConnected { get { return this.Channel != null; } }

        //Returns the URI of the node that's connected.
        public string ConnectedNode { get; private set; }

        public Seed ConnectionSeed { get; private set; }

        public string ChannelKey { get; private set; }

        public string Root { get; set; }

        //If you wish to cache the invalid nodes to prevent reattempts and improve efficiency, this list may save you some time.
        public List<string> InvalidNodesReceived { get; private set; } = new List<string>();
        #endregion


        #region Constructors.
        public IotaMamConnection(params string[] Nodes)
        {
            if (InvalidNodes(Nodes))
                throw new ArgumentException("No nodes received!");
            SetUp(Nodes);
        }
        public IotaMamConnection(IEnumerable<string> Nodes) : this(Nodes.ToArray()) { }

        public IotaMamConnection(int Timeout, params string[] Nodes)
        {
            if (InvalidNodes(Nodes))
                throw new ArgumentException("No nodes received!");
            SetUp(Nodes);
            this.Timeout = Timeout;
        }
        public IotaMamConnection(int Timeout, IEnumerable<string> Nodes) : this(Timeout, Nodes.ToArray()) { }

        public IotaMamConnection(Mode mode, params string[] Nodes)
        {
            if (InvalidNodes(Nodes))
                throw new ArgumentException("No nodes received!");
            this.ChannelMode = mode;
            SetUp(Nodes);
        }
        public IotaMamConnection(Mode mode, IEnumerable<string> Nodes) : this(mode, Nodes.ToArray()) { }

        public IotaMamConnection(Seed Seed, params string[] Nodes)
        {
            if (InvalidNodes(Nodes))
                throw new ArgumentException("No nodes received!");
            this.ConnectionSeed = Seed;
            SetUp(Nodes);
        }
        public IotaMamConnection(Seed Seed, IEnumerable<string> Nodes) : this(Seed, Nodes.ToArray()) { }
        #endregion


        #region Private work.
        private bool InvalidNodes(params string[] Nodes)
        {
            return Nodes == null || !Nodes.Any() || Nodes.All(n => string.IsNullOrWhiteSpace(n));
        }
        private bool InvalidNodes(IEnumerable<string> Nodes)
        {
            return Nodes == null || !Nodes.Any() || Nodes.All(n => string.IsNullOrWhiteSpace(n));
        }
        private void SetUp(IEnumerable<string> Nodes)
        {
            foreach (var n in Nodes)
            {
                Factory = TryCreateRepo(n);
                if (Factory != null)
                {
                    SetInnerProps(n);
                    break;
                }
            }
        }
        private IIotaRepository TryCreateRepo(string Node)
        {
            RestIotaRepository Repo = null;
            try
            { 
                Repo = IotaRepositoryFactory.Create(Node, PoW, Timeout);
            }
            catch (Exception)
            {
                InvalidNodesReceived.Add(Node);
            }
            return Repo;
        }
        private void SetInnerProps(string Node)
        {
            //If the connection seed is NULL, it means we got here from a seedless constructor, so we should generate it.
            this.ConnectionSeed = this.ConnectionSeed ?? Seed.Random();
            this.ChannelKey = Seed.Random().Value;
            this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, Factory);
            this.SubscriptionFactory = new MamChannelSubscriptionFactory(Factory, CurlMamParser.Default, CurlMask.Default);
            this.Channel = this.ChannelFactory.Create(ChannelMode, ConnectionSeed, SecurityLevel.Medium, ChannelKey);
            this.ConnectedNode = Node;
        }
        private void CreateSubscription(MaskedAuthenticatedMessage message)
        {
            this.FirstMessage = message;
            var subcriptionFactory = new MamChannelSubscriptionFactory(Factory, CurlMamParser.Default, CurlMask.Default);
            this.Subscription = subcriptionFactory.Create(FirstMessage.Root, ChannelMode, ChannelKey);
        }
        private List<UnmaskedAuthenticatedMessage> GetMessages()
        {
            var publishedMessages = new List<UnmaskedAuthenticatedMessage>();
            using (var A = AsyncHelper.Wait)
            {
                A.Run(this.Subscription.FetchAsync(), res => publishedMessages = res);
            }
            return publishedMessages;
        }
        private List<UnmaskedAuthenticatedMessage> GetMessages(string Root, string ChannelKey)
        {
            var channelSubscription = this.SubscriptionFactory.Create(new Hash(Root), Mode.Restricted, ChannelKey);
            var publishedMessages = new List<UnmaskedAuthenticatedMessage>();
            using (var A = AsyncHelper.Wait)
            {
                A.Run(channelSubscription.FetchAsync(), res => publishedMessages = res);
            }
            return publishedMessages;
        }
        #endregion

        #region Write.
        public async Task WriteAsync(string Message)
        {
            var message = Channel.CreateMessage(TryteString.FromUtf8String(Message));
            if (FirstMessage == null)
                CreateSubscription(message);
            await Channel.PublishAsync(message);
            this.Root = message.Root.Value;
        }
        public void Write(string Message)
        {
            var message = Channel.CreateMessage(TryteString.FromUtf8String(Message));
            using (var A = AsyncHelper.Wait)
            {
                A.Run(Channel.PublishAsync(message));
            }
            if (FirstMessage == null)
                CreateSubscription(message);
            this.Root = message.Root.Value;
        }
        public string WriteAndGetState(string Message)
        {
            var message = Channel.CreateMessage(TryteString.FromUtf8String(Message));
            using (var A = AsyncHelper.Wait)
            {
                A.Run(Channel.PublishAsync(message));
            }
            if (FirstMessage == null)
                CreateSubscription(message);
            this.Root = message.Root.Value;
            return JsonConvert.SerializeObject(Channel);
        }
        public async Task<string> WriteAndGetStateAsync(string Message)
        {
            var message = Channel.CreateMessage(TryteString.FromUtf8String(Message));
            await Channel.PublishAsync(message);
            if (FirstMessage == null)
                CreateSubscription(message);
            this.Root = message.Root.Value;
            return JsonConvert.SerializeObject(Channel);
        }
        #endregion

        #region GetPublishedMessages
        public async Task<List<string>> GetPublishedMessagesAsync()
        {
            var publishedMessages = await this.Subscription.FetchAsync();
            return publishedMessages.Select(x => x.Message.ToUtf8String()).ToList();
        }
        public List<string> GetPublishedMessages()
        {
            var publishedMessages = GetMessages();
            return publishedMessages.Select(x => x.Message.ToUtf8String()).ToList();
        }
        public List<string> GetPublishedMessages(string Root, string ChannelKey)
        {
            var publishedMessages = GetMessages(Root, ChannelKey);
            return publishedMessages.Select(x => x.Message.ToUtf8String()).ToList();
        }
        #endregion

        #region GetLastMessage
        public string GetLastMessage()
        {
            var publishedMessages = GetMessages();
            return publishedMessages.Last().Message.ToUtf8String();
        }
        public async Task<string> GetLastMessageAsync()
        {
            var publishedMessages = await this.Subscription.FetchAsync();
            return publishedMessages.LastOrDefault().Message.ToUtf8String();
        }
        #endregion

        #region GetFirstMessage
        public string GetFirstMessage()
        {
            var publishedMessages = GetMessages();
            return publishedMessages.FirstOrDefault()?.Message.ToUtf8String();
        }
        public async Task<string> GetFirstMessageAsync()
        {
            var publishedMessages = await this.Subscription.FetchAsync();
            return publishedMessages.FirstOrDefault()?.Message.ToUtf8String();
        }
        #endregion

    }
}
