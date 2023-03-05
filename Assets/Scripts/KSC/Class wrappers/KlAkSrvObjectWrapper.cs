using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkSrvObjectWrapper : KlAkBaseWrapper
    {
        public override string OriginalTypeName => "KlAkSrvObject";

        protected KlAkSrvObjectWrapper() { }

        public new static async Task<KlAkSrvObjectWrapper> CreateAsync()
        {
            var createdObj = new KlAkSrvObjectWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkSrvObjectWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkSrvObjectWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<KlAkProxyWrapper> GetAdmServerAsync()
            => KlAkProxyWrapper.WrapObjectWithID(await NamedPipeClient.GetPropertyFromObjectOnServerAndReturnIDAsync(ObjectID, "AdmServer"));

        public async Task SetAdmServerAsync(KlAkProxyWrapper klAkProxy)
            => await NamedPipeClient.SetPropertyToObjectOnServerAsync(ObjectID, "AdmServer", klAkProxy.ObjectID);

        public async Task<object> GetPropAsync(string strName)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(ObjectID, "get_Prop", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>()
                {
                    {0, strName}
                }
            });

        public async Task SetPropAsync(string strName, object pData)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "set_Prop", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>()
                {
                    {0, strName}
                },

                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {1, new BoxedObjectWrapper(pData)}
                }
            });
    }
}
