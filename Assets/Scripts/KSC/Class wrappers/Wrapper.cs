using NamedPipeConnection;
using System;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public abstract class Wrapper
    {
        public Guid ObjectID { get; protected set; } = Guid.NewGuid();
        public abstract string OriginalTypeName { get; }

        public async Task DestroyAsync()
        {
            await NamedPipeClient.DestroyObjectOnServerAsync(ObjectID);
        }
    }
}
