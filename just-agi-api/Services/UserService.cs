using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.Services
{
    /// <summary>
    /// �û�����
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AddUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> InvalidateUserAsync(long id)
        {
            return await _userRepository.InvalidateUserAsync(id);
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetUserListAsync()
        {
            return await _userRepository.GetUserListAsync();
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUserListAsync();
        }
    }
}
