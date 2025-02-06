using Common.Model.Global;
using Common.Model.Global.Roles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Queries.Module;
using UserServices.Application.Queries.Roles;
using UserServices.Interfaces;

namespace UserServices.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly IMediator _mediator;
        public ModuleService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<List<ModuleGVM>> GetModulesAsync()
        {
            var result = await _mediator.Send(new GetModulesQuery());

            if (result is null)
            {
                result = new List<ModuleGVM>();
            }

            return result;
            //throw new NotImplementedException();
        }
    }
}
