using Common.Model.Global;
using Common.Model.Global.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Interfaces
{
    public interface IModuleService
    {
        public Task<List<ModuleGVM>> GetModulesAsync();
    }
}
