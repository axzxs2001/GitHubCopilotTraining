using SmartAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Services
{
    /// <summary>
    /// Service interface for contract operations
    /// </summary>
    public interface IContractService
    {
        /// <summary>
        /// Gets all contracts
        /// </summary>
        Task<IEnumerable<Contract>> GetContractsAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// Gets a contract by ID
        /// </summary>
        Task<Contract> GetContractAsync(int id, CancellationToken cancellationToken);
        
        /// <summary>
        /// Adds a new contract
        /// </summary>
        Task<int> AddContractAsync(Contract contract, CancellationToken cancellationToken);
        
        /// <summary>
        /// Modifies an existing contract
        /// </summary>
        Task<bool> ModifyContractAsync(Contract contract, CancellationToken cancellationToken);
        
        /// <summary>
        /// Removes a contract by ID
        /// </summary>
        Task<bool> RemoveContractAsync(int id, CancellationToken cancellationToken);
    }
}