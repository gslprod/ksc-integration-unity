using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkParamsWrapper : KlAkBaseWrapper
    {
        public override string OriginalTypeName => "KlAkParams";

        protected KlAkParamsWrapper() { }

        public static new async Task<KlAkParamsWrapper> CreateAsync()
        {
            var createdObj = new KlAkParamsWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkParamsWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkParamsWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<long> GetCountAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<long>(ObjectID, "Count");

        public async Task AddAsync(object index, object pVal)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "Add", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(index)},
                    {1, new BoxedObjectWrapper(pVal)}
                }
            });

        public async Task RemoveAsync(object index)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "Remove", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(index)}
                }
            });

        public async Task<bool> CheckAsync(object index)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync<bool>(ObjectID, "Check", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(index)}
                }
            });

        public async Task<KlAkParamsWrapper> CloneAsync()
            => WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "Clone", null));

        public async Task<object> GetItemAsync(object index)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(ObjectID, "get_Item", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(index)}
                }
            });

        public async Task SetItemAsync(object index, object pVal)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "set_Item", new ParametersDataContainer()
            {
                BoxedObjects = new Dictionary<int, BoxedObjectWrapper>()
                {
                    {0, new BoxedObjectWrapper(index)},
                    {1, new BoxedObjectWrapper(pVal)}
                }
            });
    }
}
