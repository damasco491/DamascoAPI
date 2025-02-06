using Common.Constants;
using Common.Model.Global;
using Common.Model.Global.Roles;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Queries.Roles;
using UserServices.Application.Queries.Module;

namespace UserServices.Application.Queries.Module
{

    public class GetModulesQuery : IRequest<List<ModuleGVM>>
    {
        public GetModulesQuery() { }
    }
    public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, List<ModuleGVM>>
    {
        private readonly IRepositoryService _repositoryService;
        public GetModulesQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<List<ModuleGVM>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            var results = await _repositoryService.GetJsonPathAsyncV2<List<ModuleGVM>>(StoredProcedures.ModuleSP.GET_ALL_MODULES, dbParams);

            return results;

        }
    }

}
