using NamedPipeConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSC.Wrappers
{
    public class KlAkUsersWrapper : KlAkSrvObjectWrapper
    {
        public override string OriginalTypeName => "KlAkUsers";

        protected KlAkUsersWrapper() { }

        public new static async Task<KlAkUsersWrapper> CreateAsync()
        {
            var createdObj = new KlAkUsersWrapper();
            await NamedPipeClient.CreateObjectOnServerAsync(createdObj.OriginalTypeName, createdObj.ObjectID);

            return createdObj;
        }

        public static new KlAkUsersWrapper WrapObjectWithID(Guid objectID)
        {
            var createdObj = new KlAkUsersWrapper
            {
                ObjectID = objectID
            };

            return createdObj;
        }

        public async Task<long> AddUserAsync(KlAkParamsWrapper pUser)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync<long>(ObjectID, "AddUser", new ParametersDataContainer()
            {
                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {0, pUser.ObjectID}
                }
            });

        public async Task UpdateUserAsync(long lUserId, KlAkParamsWrapper pUser)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "UpdateUser", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>
                {
                    {0, lUserId}
                },

                KlAkObjectParameters = new Dictionary<int, System.Guid>
                {
                    {1, pUser.ObjectID}
                }
            });

        public async Task<KlAkParamsWrapper> GetUserAsync(long lUserId)
            => KlAkParamsWrapper.WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "GetUser", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>
                {
                    {0, lUserId}
                },
            }));

        public async Task<KlAkCollectionWrapper> GetUsersAsync()
            => KlAkCollectionWrapper.WrapObjectWithID(await NamedPipeClient.ExecuteObjectMethodOnServerAndReturnIDAsync(ObjectID, "GetUsers", null));

        public async Task ChangeUserPasswordAsync(long lUserId, string bstrOldPassword, string bstrNewPassword)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "ChangeUserPassword", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>
                {
                    {0, lUserId}
                },

                StringParameters = new Dictionary<int, string>
                {
                    {1, bstrOldPassword},
                    {2, bstrNewPassword}
                }
            });

        public async Task DeleteUserAsync(long lUserId)
            => await NamedPipeClient.ExecuteObjectMethodOnServerAsync(ObjectID, "DeleteUser", new ParametersDataContainer()
            {
                NumParameters = new Dictionary<int, long>
                {
                    {0, lUserId}
                },
            });
    }
}
