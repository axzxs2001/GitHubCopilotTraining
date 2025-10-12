using Microsoft.Extensions.Logging;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Services
{
    public class ContractService : IContractService
    {
        readonly IContractRepository _contractRepository;
        readonly ILogger<ContractService> _logger;
        
        public ContractService(ILogger<ContractService> logger, IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<Contract>> GetContractsAsync(CancellationToken cancellationToken)
        {
            return await _contractRepository.GetContractsAsync(cancellationToken);
        }
        
        public async Task<Contract> GetContractAsync(int id, CancellationToken cancellationToken)
        {
            return await _contractRepository.GetContractAsync(id, cancellationToken);
        }
        
        public async Task<int> AddContractAsync(Contract contract, CancellationToken cancellationToken)
        {
            contract.CreateTime = DateTime.Now;
            contract.ModifyTime = DateTime.Now;
            contract.CreateUser = "sys";
            contract.ModifyUser = "sys";
            
            return await _contractRepository.CreateContractAsync(contract, cancellationToken);
        }
        
        public async Task<bool> ModifyContractAsync(Contract contract, CancellationToken cancellationToken)
        {
            contract.ModifyTime = DateTime.Now;
            contract.ModifyUser = "sys";
            
            var result = await _contractRepository.UpdateContractAsync(contract, cancellationToken);
            return result > 0;
        }
        
        public async Task<bool> RemoveContractAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _contractRepository.DeleteContractAsync(id, cancellationToken);
            return result > 0;
        }
    }
}