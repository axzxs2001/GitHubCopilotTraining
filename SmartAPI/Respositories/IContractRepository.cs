using SmartAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Respositories
{
    /// <summary>
    /// Repository interface for Contract entity operations
    /// </summary>
    public interface IContractRepository
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
        /// Creates a new contract
        /// </summary>
        Task<int> CreateContractAsync(Contract contract, CancellationToken cancellationToken);
        
        /// <summary>
        /// Updates an existing contract
        /// </summary>
        Task<int> UpdateContractAsync(Contract contract, CancellationToken cancellationToken);
        
        /// <summary>
        /// Deletes a contract by ID
        /// </summary>
        Task<int> DeleteContractAsync(int id, CancellationToken cancellationToken);
    }
}