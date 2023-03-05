using KSC.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace KSC
{
    public class Server
    {
        #region Static

        public static event Action<Server> OnServerRegistered;
        public static event Action<Server> OnServerUnregistered;

        public static Server[] RegisteredServers => _registeredServers.ToArray();

        private static List<Server> _registeredServers = new List<Server>();

        #endregion

        public event Action<Server> OnDispose;
        public event Action<Server> OnInfoUpdated;

        public KlAkProxyWrapper AdmServer { get; private set; }
        public KlAkHostsWrapper HostsProvider { get; private set; }
        public KlAkUsersWrapper UsersProvider { get; private set; }

        public string IPAddress { get; private set; }
        public string ServerName { get; private set; }
        public string UserName { get; private set; }
        public Agent[] Agents => _agents.ToArray();

        private List<Agent> _agents = new List<Agent>();

        private Server(KlAkProxyWrapper admServer, KlAkHostsWrapper hostsProvider, KlAkUsersWrapper usersProvider, string userName, string ip)
        {
            UserName = userName;
            IPAddress = ip;
            AdmServer = admServer;

            HostsProvider = hostsProvider;
            UsersProvider = usersProvider;
        }

        public static async Task<Server> ConnectAndReturnAsync(string address, bool useSSL, string user, string domain, string password, bool register = true)
        {
            var proxyCreationTask = KlAkProxyWrapper.CreateAsync();
            var parametersCreationTask = KlAkParamsWrapper.CreateAsync();
            var hostsProviderCreationTask = KlAkHostsWrapper.CreateAsync();
            var usersProviderCreationTask = KlAkUsersWrapper.CreateAsync();

            KlAkProxyWrapper proxy = await proxyCreationTask;
            KlAkParamsWrapper parameters = await parametersCreationTask;
            KlAkHostsWrapper hostsProvider = await hostsProviderCreationTask;
            KlAkUsersWrapper usersProvider = await usersProviderCreationTask;

            var addParamsTasks = new Task[]
            {
                parameters.AddAsync("Address", address),
                parameters.AddAsync("UseSSL", useSSL),
                parameters.AddAsync("Domain", domain),
                parameters.AddAsync("User", user),
                parameters.AddAsync("Password", password)
            };

            await Task.WhenAll(addParamsTasks);

            await proxy.ConnectAsync(parameters);

            var admServerSettingTasks = new Task[]
            {
                hostsProvider.SetAdmServerAsync(proxy),
                usersProvider.SetAdmServerAsync(proxy)
            };

            await Task.WhenAll(admServerSettingTasks);

            var server = new Server(proxy, hostsProvider, usersProvider, user, address);
            if (register)
                RegisterServer(server);

            return server;
        }

        public async Task DisposeAsync()
        {
            var disposeTasks = new Task[]
            {
                DeleteAllAgentsAsync(),
                AdmServer.DisconnectAsync()
            };

            await Task.WhenAll(disposeTasks);

            if (_registeredServers.Contains(this))
                UnregisterServer(this);

            OnDispose?.Invoke(this);
        }

        public async Task UpdateInfoAsync()
        {
            var updateTasks = new Task[]
            {
                UpdateServerInfoAsync(),
                UpdateHostsInfoAsync()
            };

            await Task.WhenAll(updateTasks);
            OnInfoUpdated?.Invoke(this);
        }

        private static void RegisterServer(Server server)
        {
            _registeredServers.Add(server);

            OnServerRegistered?.Invoke(server);
        }

        private static void UnregisterServer(Server server)
        {
            _registeredServers.Remove(server);

            OnServerUnregistered?.Invoke(server);
        }

        private async Task UpdateServerInfoAsync()
        {
            var getServerHostNameTask = AdmServer.GetPropAsync(PropertiesOfTheConnection.KLADMSRV_SERVER_HOSTNAME);

            KlAkCollectionWrapper toReturn = await KlAkCollectionWrapper.CreateAsync();
            await toReturn.SetSizeAsync(1);
            await toReturn.SetAtAsync(0, "KLHST_WKS_DN");

            var serverHostName = (string)await getServerHostNameTask;
            var getServerInfoTask = HostsProvider.GetHostInfoAsync(serverHostName, toReturn);

            var serverInfo = await getServerInfoTask;
            ServerName = (string)await serverInfo.GetItemAsync("KLHST_WKS_DN");
        }

        private async Task UpdateHostsInfoAsync()
        {
            var createToReturnTask = KlAkCollectionWrapper.CreateAsync();
            var createSortingByNameTask = KlAkParamsWrapper.CreateAsync();
            var createSortingByStatusTask = KlAkParamsWrapper.CreateAsync();
            var createToSortTask = KlAkCollectionWrapper.CreateAsync();

            KlAkCollectionWrapper toReturn = await createToReturnTask;
            await toReturn.SetSizeAsync(6);
            var setToReturnTasks = new Task[]
            {
                toReturn.SetAtAsync(0, "KLHST_WKS_IP"),
                toReturn.SetAtAsync(1, "KLHST_WKS_DN"),
                toReturn.SetAtAsync(2, "KLHST_WKS_STATUS_ID"),
                toReturn.SetAtAsync(3, "KLHST_WKS_HOSTNAME"),
                toReturn.SetAtAsync(4, "name"),
                toReturn.SetAtAsync(5, "KLHST_WKS_STATUS")
            };

            await Task.WhenAll(setToReturnTasks);

            KlAkParamsWrapper sortingByName = await createSortingByNameTask;
            var addToSortingByNameParamsTasks = new Task[]
            {
                sortingByName.AddAsync("Name", "KLHST_WKS_DN"),
                sortingByName.AddAsync("Asc", true)
            };

            await Task.WhenAll(addToSortingByNameParamsTasks);

            KlAkParamsWrapper sortingByStatus = await createSortingByStatusTask;
            var addToSortingByStatusParamsTasks = new Task[]
            {
                sortingByName.AddAsync("Name", "KLHST_WKS_STATUS_ID"),
                sortingByName.AddAsync("Asc", true)
            };

            await Task.WhenAll(addToSortingByStatusParamsTasks);

            KlAkCollectionWrapper toSort = await createToSortTask;
            await toSort.SetSizeAsync(2);
            var setToSortTasks = new Task[]
            {
                toSort.SetAtAsync(0, sortingByName),
                toSort.SetAtAsync(1, sortingByStatus)
            };

            var hosts = await HostsProvider.FindHostsAsync("(KLHST_WKS_STATUS & 4 = 4)", toReturn, toSort);
            var hostsCollection = await hosts.GetChunkAsync(0, await hosts.GetCountAsync());

            await UpdateAndCreateAgentsAsync(hostsCollection);
            await DeleteNotValidAgentsAsync(hostsCollection);
        }

        private async Task UpdateAndCreateAgentsAsync(KlAkCollectionWrapper updateSource)
        {
            var count = await updateSource.GetCountAsync();
            for (int i = 0; i < count; i++)
            {
                var hostInfo = (KlAkParamsWrapper)await updateSource.GetItemAsync(i);

                var getHostNameTask = hostInfo.GetItemAsync("KLHST_WKS_HOSTNAME");
                var getNameTask = hostInfo.GetItemAsync("KLHST_WKS_DN");
                var getGroupTask = hostInfo.GetItemAsync("name");
                var getStatusTask = hostInfo.GetItemAsync("KLHST_WKS_STATUS_ID");
                var getIpIntTask = hostInfo.GetItemAsync("KLHST_WKS_IP");

                var hostName = (string)await getHostNameTask;
                var name = (string)await getNameTask;
                var group = (string)await getGroupTask;
                var status = (int)await getStatusTask;
                var ipInt = (int)await getIpIntTask;

                var ip = ipInt.FromLittleEndianByteOrderToIP();

                Agent agent;
                if ((agent = _agents.Find((existAgent) => existAgent.HostName == hostName)) == null)
                {
                    agent = new Agent(hostName);
                    _agents.Add(agent);
                }

                agent.UpdateAgentInfo(name, group, ip, (AgentStatus)status);
            }
        }

        private async Task DeleteNotValidAgentsAsync(KlAkCollectionWrapper updateSource)
        {
            var count = await updateSource.GetCountAsync();

            string[] hostNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                var hostInfo = (KlAkParamsWrapper)await updateSource.GetItemAsync(i);
                hostNames[i] = (string)await hostInfo.GetItemAsync("KLHST_WKS_HOSTNAME");
            }

            foreach (var existAgent in _agents)
            {
                if (!Array.Exists(hostNames, (hostName) => hostName == existAgent.HostName))
                {
                    _agents.Remove(existAgent);
                    existAgent.Dispose();
                }
            }
        }

        private async Task DeleteAllAgentsAsync()
        {
            await Task.Run(() =>
            {
                foreach (var agent in _agents)
                    agent.Dispose();

                _agents.Clear();
            });
        }
    }
}
