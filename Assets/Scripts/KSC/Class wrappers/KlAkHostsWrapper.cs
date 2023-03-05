using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkHostsWrapper : KlAkSrvObjectWrapper
    {
        public override string OriginalTypeName => "KlAkHosts";

        protected KlAkHostsWrapper() { }

        public new static async Task<KlAkHostsWrapper> CreateAsync()
        {
            var createdObj = new KlAkHostsWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkHostsWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkHostsWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<string> AddHostAsync(KlAkParamsWrapper pInfo)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync<string>(ObjectID, "AddHost", new ParametersDataContainer()
            {
                KlAkObjectParameters = new Dictionary<int, System.Guid>()
                {
                    {0, pInfo.ObjectID}
                }
            });

        public async Task UpdateHostAsync(string strHostName, KlAkParamsWrapper pInfo)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "UpdateHost", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>
                {
                    {0, strHostName}
                },

                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {1, pInfo.ObjectID}
                }
            });

        public async Task RemoveHostAsync(string strHostName)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "RemoveHost", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>
                {
                    {0, strHostName}
                }
            });

        public async Task<KlAkParamsWrapper> GetHostInfoAsync(string strHostName, KlAkCollectionWrapper pFields2Return)
            => KlAkParamsWrapper.WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "GetHostInfo", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>
                {
                    {0, strHostName}
                },

                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {1, pFields2Return.ObjectID}
                }
            }));

        public async Task<KlAkChunkAccessorWrapper> FindHostsAsync(string strFilter, KlAkCollectionWrapper pFields2Return, KlAkCollectionWrapper pSortFields)
            => KlAkChunkAccessorWrapper.WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "FindHosts", new ParametersDataContainer()
            {
                StringParameters = new Dictionary<int, string>
                {
                    {0, strFilter}
                },

                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {1, pFields2Return.ObjectID},
                    {2, pSortFields.ObjectID}
                }
            }));

        public async Task MoveHostsToGroupAsync(long nGroup, KlAkCollectionWrapper pHostNames)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "MoveHostsToGroup", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>
                {
                    {0, nGroup},
                },

                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {1, pHostNames.ObjectID}
                }
            });


        public async Task RemoveHostsAsync(KlAkCollectionWrapper pHostNames, bool bForceDestroy)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "RemoveHosts", new ParametersDataContainer()
            {
                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {0, pHostNames.ObjectID}
                },

                BoolParameters = new Dictionary<int, bool>
                {
                    {1, bForceDestroy}
                }
            });

        public async Task ZeroVirusCountForGroupAsync(long nParent)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "ZeroVirusCountForGroup", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>()
                {
                    {0, nParent}
                }
            });

        public async Task ZeroVirusCountForHostsAsync(KlAkCollectionWrapper pHostNames)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "ZeroVirusCountForHosts", new ParametersDataContainer()
            {
                KlAkObjectParameters = new Dictionary<int, System.Guid>()
                {
                    {0, pHostNames.ObjectID}
                }
            });
    }
}
