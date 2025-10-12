using Microsoft.Extensions.Logging;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Services
{
    public class ContractUserService : IContractUserService
    {
        readonly IContractUserRepository _contractUserRepository;
        readonly ILogger<ContractUserService> _logger;

        public ContractUserService(ILogger<ContractUserService> logger, IContractUserRepository contractUserRepository)
        {
            _contractUserRepository = contractUserRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ContractUser>> GetContractUsersAsync(CancellationToken cancellationToken)
        {
            return await _contractUserRepository.GetContractUsersAsync(cancellationToken);
        }

        public async Task<ContractUser> GetContractUserAsync(int id, CancellationToken cancellationToken)
        {
            return await _contractUserRepository.GetContractUserAsync(id, cancellationToken);
        }

        public async Task<int> AddContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken)
        {
            contractUser.CreateTime = DateTime.Now;
            return await _contractUserRepository.CreateContractUserAsync(contractUser, cancellationToken);
        }

        public async Task<bool> ModifyContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken)
        {
            var result = await _contractUserRepository.UpdateContractUserAsync(contractUser, cancellationToken);
            return result > 0;
        }

        public async Task<bool> RemoveContractUserAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _contractUserRepository.DeleteContractUserAsync(id, cancellationToken);
            return result > 0;
        }
        /// <summary>
        /// Checks if a user has signed the latest contract
        /// </summary>
        /// <param name="user">The user ID or username</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The contract if the user has signed the latest contract, null otherwise</returns>
        public async Task<Contract> HasUserSignedLatestContractAsync(string user, CancellationToken cancellationToken)
        {
            return await _contractUserRepository.HasUserSignedLatestContractAsync(user, cancellationToken);
        }


    }
}