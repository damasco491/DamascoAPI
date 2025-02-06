using Common.Constants;
using Common.Model;
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
    public class GetRoleByIdQuery : IRequest<RoleGVM>
    {
        public string RoleId { get; set; }
        public GetRoleByIdQuery(string roleId)
        {
            RoleId = roleId;
        }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleGVM>
    {
        private readonly IRepositoryService _repositoryService;
        public GetRoleByIdQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<RoleGVM> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            dbParams.Add("roleId", request.RoleId);
            var results = await _repositoryService.GetJsonPathAsyncV2<RoleGVM>(StoredProcedures.RoleSP.GET_ROLE_BY_ID, dbParams);

            return results;

        }
    }
}
