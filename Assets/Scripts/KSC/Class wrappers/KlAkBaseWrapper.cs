using NamedPipeConnection;
using System;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkBaseWrapper : Wrapper
    {
        public override string OriginalTypeName => "KlAkBase";

        protected KlAkBaseWrapper() { }

        public static async Task<KlAkBaseWrapper> CreateAsync()
        {
            var createdObj = new KlAkBaseWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static KlAkBaseWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkBaseWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<int> GetTypeAsync()
            => await NamedPipeClient.GetPropertyFromObjectOnServerAsync<int>(ObjectID, "Type");
    }
}
