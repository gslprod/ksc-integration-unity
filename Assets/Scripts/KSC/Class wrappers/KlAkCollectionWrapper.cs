using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkCollectionWrapper : KlAkBaseWrapper
    {
        public override string OriginalTypeName => "KlAkCollection";

        protected KlAkCollectionWrapper() { }

        public new static async Task<KlAkCollectionWrapper> CreateAsync()
        {
            var createdObj = new KlAkCollectionWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkCollectionWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkCollectionWrapper
            {
                ObjectID = objectID
            };
            return createdObj;
        }

        public async Task<long> GetCountAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<long>(ObjectID, "Count");

        public async Task SetSizeAsync(long nNewSize)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "SetSize", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>()
                {
                    {0, nNewSize}
                }
            });

        public async Task<KlAkCollectionWrapper> CloneAsync()
            => WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "Clone", null));

        public async Task SetAtAsync(long nID, object pVal)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "SetAt", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>()
                {
                    {0, nID}
                },

                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {1, new BoxedObjectWrapper(pVal)}
                }
            });

        public async Task<object> GetItemAsync(long nID)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(ObjectID, "get_Item", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(nID)}
                }
            });

        public async Task SetItemAsync(long nID, object pVal)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "set_Item", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>()
                {
                    {0, nID}
                },

                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {1, new BoxedObjectWrapper(pVal)}
                }
            });
    }
}
