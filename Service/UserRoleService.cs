using BusinessObject.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _roleRepository;

        public UserRoleService(IUserRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<string> CheckRole(User user)
        {
            return await _roleRepository.CheckRole(user);
        }
    }
}
