using Common.Constants;
using Common.Model;
using Common.Model.Global.Roles;
using Common.ViewModels;
using Dapper;
using Datalayer;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Application.Commands.Roles
{
    public class CreateRoleCommand : IRequest<CustomModelResponse>
    {
        public RoleGVM Role { get; set; }
        public CreateRoleCommand(RoleGVM postData)
        {
            Role = postData;
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CustomModelResponse>
    {
        private readonly IRepositoryService _repositoryService;
        public CreateRoleCommandHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<CustomModelResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var response = new CustomModelResponse(ApiResponseMessage.Exceptions.INTERNAL_SERVER_MESSAGE, 500, false);
            try
            {
                var moduleAccessJsonRes = JsonConvert.SerializeObject(request.Role.RoleModuleAccess);

                var dbParams = new DynamicParameters();
                dbParams.Add("lineId", request.Role.LineId);
                dbParams.Add("roleId", request.Role.RoleId);
                dbParams.Add("name", request.Role.Name);
                dbParams.Add("description", request.Role.Description);
                dbParams.Add("isActive", request.Role.IsActive);
                dbParams.Add("createdBy", request.Role.CreatedBy);

                dbParams.Add("roleModuleAccess", moduleAccessJsonRes);

                var results = _repositoryService.ExecuteWithReturnAsync<string>(StoredProcedures.RoleSP.CREATE_ROLE, dbParams);

                response = new CustomModelResponse(ApiResponseMessage.SUCCESS, 200, true, results.Result.ToString());
            }
            catch (Exception ex)
            {
                response = new CustomModelResponse(ex.InnerException.Message, 500, false);
            }
            
            return response;

        }
    }
}
