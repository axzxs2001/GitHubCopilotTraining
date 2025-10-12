using SmartAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Respositories
{
    public interface IContractUserRepository
    {
        Task<IEnumerable<ContractUser>> GetContractUsersAsync(CancellationToken cancellationToken);
        Task<ContractUser> GetContractUserAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken);
        Task<int> UpdateContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken);
        Task<int> DeleteContractUserAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a user has signed the latest contract
        /// </summary>
        /// <param name="user">The user ID or username</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The contract if the user has signed the latest contract, null otherwise</returns>
        Task<Contract> HasUserSignedLatestContractAsync(string user, CancellationToken cancellationToken);

    }
}