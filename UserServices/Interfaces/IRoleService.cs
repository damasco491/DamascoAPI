using Common.Model.Global.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices
{
    public interface IRoleService
    {
        public Task<List<RoleGVM>> GetRolesAsync();
        public Task<RoleGVM> GetRoleByIdAsync(string roleId);
        public Task<RoleGVM> CreateRoleAsync(RoleGVM role);
        public Task<RoleGVM> UpdateRoleAsync(RoleGVM role);
    }
}
