using BusinessObject;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
    {


        public UserRoleRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<string> CheckRole(User user)
        {
           return await _db.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.UserId)
            .Select(ur => ur.Role.RoleName)
            .FirstOrDefaultAsync();
        }
    }
}
