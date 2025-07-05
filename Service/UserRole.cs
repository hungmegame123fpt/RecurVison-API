using BusinessObject.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserRole
    {
        private readonly IUserRoleRepository _roleRepository;

        public UserRole(IUserRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<string> CheckRole(User user)
        {
            return await _roleRepository.CheckRole(user);
        }
    }
}
