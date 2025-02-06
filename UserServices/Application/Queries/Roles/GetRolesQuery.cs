using Common.Constants;
using Common.Model.Global.Roles;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Application.Queries.Roles
{
    public class GetRolesQuery : IRequest<List<RoleGVM>>
    {
        public GetRolesQuery() { }
    }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleGVM>>
    {
        private readonly IRepositoryService _repositoryService;
        public GetRolesQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<List<RoleGVM>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            var results = await _repositoryService.GetJsonPathAsyncV2<List<RoleGVM>>(StoredProcedures.RoleSP.GET_ALL_ROLES, dbParams);

            return results;

        }
    }
}
