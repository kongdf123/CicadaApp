using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.Collections.Concurrent;

namespace Cicada.Worker.Opc
{
    public class OpcUaClientService : IDisposable
    {
        private Session _session;
        private ApplicationConfiguration _config;
        private readonly ConcurrentDictionary<string, Subscription> _subscriptions = new();

        public bool IsConnected => _session?.Connected ?? false;

        #region 🔌 Connect

        public async Task ConnectAsync(string endpointUrl)
        {
            var endpoint = CoreClientUtils.SelectEndpoint(endpointUrl, false);

            _config = new ApplicationConfiguration()
            {
                ApplicationName = "CicadaClient",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 15000
                },
                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = 60000
                }
            };

            _config.CertificateValidator = new CertificateValidator();
            _config.CertificateValidator.CertificateValidation += (s, e) =>
            {
                e.Accept = true;
            };

            var endpointConfig = EndpointConfiguration.Create(_config);
            var configuredEndpoint = new ConfiguredEndpoint(null, endpoint, endpointConfig);

            _session = await Session.Create(
                _config,
                configuredEndpoint,
                false,
                "CicadaSession",
                60000,
                null,
                null);
            //_config = new ApplicationConfiguration
            //{
            //    ApplicationName = "Cicada OPC Client",
            //    ApplicationType = ApplicationType.Client,
            //    ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:CicadaOPCClient",
            //    SecurityConfiguration = new SecurityConfiguration
            //    {
            //        ApplicationCertificate = new CertificateIdentifier
            //        {
            //            StoreType = "Directory",
            //            StorePath = "pki/own",
            //            SubjectName = "CN=Cicada OPC Client"
            //        },
            //        TrustedPeerCertificates = new CertificateTrustList
            //        {
            //            StoreType = "Directory",
            //            StorePath = "pki/trusted"
            //        },
            //        TrustedIssuerCertificates = new CertificateTrustList
            //        {
            //            StoreType = "Directory",
            //            StorePath = "pki/issuers"
            //        },
            //        RejectedCertificateStore = new CertificateTrustList
            //        {
            //            StoreType = "Directory",
            //            StorePath = "pki/rejected"
            //        },
            //        AutoAcceptUntrustedCertificates = true,
            //        RejectSHA1SignedCertificates = false,
            //        AddAppCertToTrustedStore = true
            //    },
            //    TransportConfigurations = new TransportConfigurationCollection(),
            //    TransportQuotas = new TransportQuotas
            //    {
            //        OperationTimeout = 15000
            //    },
            //    ClientConfiguration = new ClientConfiguration
            //    {
            //        DefaultSessionTimeout = 60000
            //    }
            //};

            //await _config.Validate(ApplicationType.Client);

            //// Ensure certificate exists
            //var cert = await _config.SecurityConfiguration.ApplicationCertificate.Find(true);
            //if (cert == null)
            //{
            //    string applicationUri = _config.ApplicationUri;
            //    string applicationName = _config.ApplicationName;
            //    string subjectName = _config.SecurityConfiguration.ApplicationCertificate.SubjectName;

            //    cert = CertificateFactory.CreateCertificate(
            //        _config.SecurityConfiguration.ApplicationCertificate.StoreType,
            //        _config.SecurityConfiguration.ApplicationCertificate.StorePath,
            //        null, // password
            //        applicationUri,
            //        applicationName,
            //        subjectName,
            //        null, // domainNames
            //        2048, // keySize
            //        DateTime.UtcNow,
            //        120, // lifetime in months
            //        256, // hashSizeInBits (use SHA256)
            //        false, // isCA
            //        null, // issuerCAKeyCert
            //        null, // publicKey
            //        0     // pathLengthConstraint
            //    );

            //    _config.SecurityConfiguration.ApplicationCertificate.Certificate = cert;
            //}

            //var endpoint = CoreClientUtils.SelectEndpoint(endpointUrl, false);
            //var endpointConfig = EndpointConfiguration.Create(_config);

            //_session = await Session.Create(
            //    _config,
            //    new ConfiguredEndpoint(null, endpoint, endpointConfig),
            //    false,
            //    "CicadaSession",
            //    60000,
            //    null,
            //    null);

            //_session.KeepAlive += Session_KeepAlive;

            Console.WriteLine("✅ OPC UA Connected");
        }

        #endregion

        #region 🔄 Reconnect

        private void Session_KeepAlive(ISession sender, KeepAliveEventArgs e)
        {
            if (ServiceResult.IsBad(e.Status))
            {
                Console.WriteLine("⚠ OPC UA connection lost. Reconnecting...");
                _ = ReconnectAsync();
            }
        }

        private async Task ReconnectAsync()
        {
            try
            {
                await Task.Delay(3000);

                await _session.CloseAsync();
                _session.Dispose();

                Console.WriteLine("🔄 Reconnecting OPC UA...");
                await ConnectAsync(_session.ConfiguredEndpoint.EndpointUrl.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Reconnect failed: {ex.Message}");
            }
        }

        #endregion

        #region 📡 Subscribe Single Node

        public void SubscribeNode(string nodeId, Action<double> onData)
        {
            if (!IsConnected)
                throw new Exception("OPC UA not connected");

            var subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingInterval = 1000
            };

            var monitoredItem = new MonitoredItem(subscription.DefaultItem)
            {
                StartNodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value,
                DisplayName = nodeId,
                SamplingInterval = 1000
            };

            monitoredItem.Notification += (monitoredItem, args) =>
            {
                foreach (var value in monitoredItem.DequeueValues())
                {
                    try
                    {
                        var val = Convert.ToDouble(value.Value);
                        onData?.Invoke(val);
                    }
                    catch
                    {
                        // ignore bad data
                    }
                }
            };

            subscription.AddItem(monitoredItem);
            _session.AddSubscription(subscription);
            subscription.Create();

            _subscriptions[nodeId] = subscription;

            Console.WriteLine($"📡 Subscribed: {nodeId}");
        }

        #endregion

        #region 📡 Subscribe Multiple Nodes

        public void SubscribeNodes(IEnumerable<string> nodeIds, Action<string, double> onData)
        {
            if (!IsConnected)
                throw new Exception("OPC UA not connected");

            var subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingInterval = 1000
            };

            foreach (var nodeId in nodeIds)
            {
                var monitoredItem = new MonitoredItem(subscription.DefaultItem)
                {
                    StartNodeId = new NodeId(nodeId),
                    AttributeId = Attributes.Value,
                    DisplayName = nodeId,
                    SamplingInterval = 1000
                };

                monitoredItem.Notification += (item, args) =>
                {
                    foreach (var value in item.DequeueValues())
                    {
                        try
                        {
                            var val = Convert.ToDouble(value.Value);
                            onData?.Invoke(item.DisplayName, val);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                };

                subscription.AddItem(monitoredItem);
            }

            _session.AddSubscription(subscription);
            subscription.Create();

            Console.WriteLine("📡 Multiple nodes subscribed");
        }

        #endregion

        #region 📖 Read Once (Optional)

        public async Task<double?> ReadNodeAsync(string nodeId)
        {
            if (!IsConnected)
                return null;

            try
            {
                //var nodes = _session.FetchReferences(ObjectIds.ObjectsFolder);

                //foreach (var n in nodes)
                //{
                //    Console.WriteLine(n.DisplayName);
                //    Console.WriteLine(n.NodeId);
                //}
                //this.Browse();
                //var rootNodes = this.BrowseTree(ObjectIds.ObjectsFolder);

                var result = await _session.BrowseAsync(
                    null,
                    null,
                    new List<NodeId> { ObjectIds.ObjectsFolder },
                    0,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)BrowseResultMask.All);

                var node = NodeId.Parse(nodeId);

                var dv = await _session.ReadValueAsync(node);

                if (dv?.Value == null)
                    return null;

                if (dv.Value is IConvertible)
                    return Convert.ToDouble(dv.Value);

                return null;
                //var value = await _session.ReadValueAsync(new NodeId(nodeId));
                //return Convert.ToDouble(value.Value);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        //public async Task<List<ReferenceDescription>> BrowseAsync(NodeId root)
        //{
        //    var result = await _session.BrowseAsync(
        //        null,
        //        null,
        //        new List<NodeId> { root },
        //        0,
        //        BrowseDirection.Forward,
        //        ReferenceTypeIds.HierarchicalReferences,
        //        true,
        //        (uint)BrowseResultMask.All);

        //    return result.Results[0].References.ToList();
        //}
        public List<OpcNode> BrowseTree(NodeId root)
        {
            var result = new List<OpcNode>();

            BrowseDescription nodeToBrowse = new BrowseDescription
            {
                NodeId = ObjectIds.ObjectsFolder,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                ResultMask = (uint)BrowseResultMask.All
            };

            _session.Browse(
                null,
                null,
                0,
                new BrowseDescriptionCollection { nodeToBrowse },
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diag);

            //var nodes = _session.Browse(
            //    null,
            //    null,
            //    root,
            //    0,
            //    BrowseDirection.Forward,
            //    ReferenceTypeIds.HierarchicalReferences,
            //    true,
            //    (uint)BrowseResultMask.All,
            //    out BrowseResultCollection results,
            //    out ReferenceDescriptionCollection references);

            foreach (var r in results[0].References)
            {
                var node = new OpcNode
                {
                    DisplayName = r.DisplayName.Text,
                    NodeId = r.NodeId.ToString(),
                    NodeClass = r.NodeClass
                };

                // 只递归 Folder/Object
                if (r.NodeClass != NodeClass.Variable)
                {
                    node.Children = BrowseTree(ExpandedNodeId.ToNodeId(r.NodeId, _session.NamespaceUris));
                }

                result.Add(node);
            }

            return result;
        }
        public void Browse()
        {
            var nodeToBrowse = ObjectIds.ObjectsFolder;

            var description = new BrowseDescription
            {
                NodeId = nodeToBrowse,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                ResultMask = (uint)BrowseResultMask.All
            };

            _session.Browse(
                null,
                null,
                0,
                new BrowseDescriptionCollection { description },
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diag);

            foreach (var r in results[0].References)
            {
                Console.WriteLine($"{r.DisplayName} -> {r.NodeId}");
            }
        }
        #endregion

        #region ❤️ Heartbeat (Optional)

        public async Task StartHeartbeatAsync(string nodeId, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var value = await ReadNodeAsync(nodeId);
                Console.WriteLine($"❤️ Heartbeat {nodeId}: {value}");

                await Task.Delay(5000, token);
            }
        }

        #endregion

        #region 🧹 Dispose

        public void Dispose()
        {
            try
            {
                foreach (var sub in _subscriptions.Values)
                {
                    sub.Delete(true);
                }

                _session?.Close();
                _session?.Dispose();
            }
            catch
            {
            }
        }

        #endregion
    }
    public class OpcNode
    {
        public string DisplayName { get; set; }
        public string NodeId { get; set; }
        public NodeClass NodeClass { get; set; }
        public List<OpcNode> Children { get; set; } = new();
    }
}
