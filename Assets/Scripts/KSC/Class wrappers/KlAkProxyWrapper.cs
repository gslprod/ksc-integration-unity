using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public enum PropertiesOfTheConnection
    {
        IsAlive = 1,
        KLADMSRV_VS_LICDISABLED,
        KLADMSRV_VSID,
        KLADMSRV_USERID,
        KLADMSRV_SAAS_BLOCKED,
        KLADMSRV_SERVER_HOSTNAME,
        KLADMSRV_PUBLICKEY
    }

    public class KlAkProxyWrapper : KlAkBaseWrapper
    {
        public override string OriginalTypeName => "KlAkProxy";

        protected KlAkProxyWrapper() { }

        public static new async Task<KlAkProxyWrapper> CreateAsync()
        {
            var createdObj = new KlAkProxyWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkProxyWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkProxyWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<long> GetVersionIdAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<long>(ObjectID, "VersionId");

        public async Task<string> GetBuildAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<string>(ObjectID, "Build");

        public async Task<object> GetCertificateAsync()
            => await NamedPipeClient.GetBoxedObjectFromPropertyFromObjectOnServerAsync(ObjectID, "Certificate");

        public async Task ConnectAsync(KlAkParamsWrapper pSettings)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "Connect", new ParametersDataContainer()
            {
                KlAkObjectParameters = new Dictionary<int, Guid>
                {
                    {0, pSettings.ObjectID}
                }
            });

        public async Task DisconnectAsync()
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "Disconnect", null);

        public async Task<object> GetPropAsync(PropertiesOfTheConnection strName)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(ObjectID, "GetProp", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>()
                {
                    {0, strName.ToString() }
                }
            });

        public async Task<object> LoadCertificateAsync(string strFilename)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(ObjectID, "LoadCertificate", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>()
                {
                    {0, strFilename }
                }
            });
       
    }
}
