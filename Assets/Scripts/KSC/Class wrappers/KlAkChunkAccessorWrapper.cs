using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkChunkAccessorWrapper : Wrapper
    {
        public override string OriginalTypeName => "KlAkChunkAccessor";

        protected KlAkChunkAccessorWrapper() { }

        public static async Task<KlAkChunkAccessorWrapper> CreateAsync()
        {
            var createdObj = new KlAkChunkAccessorWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static KlAkChunkAccessorWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkChunkAccessorWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<long> GetCountAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<long>(ObjectID, "Count");

        public async Task<KlAkCollectionWrapper> GetChunkAsync(long lStart, long lCount)
            => KlAkCollectionWrapper.WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "GetChunk", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>()
                {
                    {0, lStart},
                    {1, lCount}
                }
            }));
    }
}
