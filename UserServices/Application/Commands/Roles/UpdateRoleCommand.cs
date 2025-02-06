using Common.Constants;
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
    public class UpdateRoleCommand : IRequest<CustomModelResponse>
    {
        public RoleGVM Role { get; set; }
        public UpdateRoleCommand(RoleGVM postData)
        {
            Role = postData;
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, CustomModelResponse>
    {
        private readonly IRepositoryService _repositoryService;
        public UpdateRoleCommandHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<CustomModelResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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
                dbParams.Add("updatedBy", request.Role.UpdatedBy);
                dbParams.Add("roleModuleAccess", moduleAccessJsonRes);

                await _repositoryService.ExecuteAsync(StoredProcedures.RoleSP.UPDATE_ROLE, dbParams);
                response = new CustomModelResponse(ApiResponseMessage.SUCCESS, 200, true);
            }
            catch (Exception ex)
            {
                var errMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                response = new CustomModelResponse(errMsg, 500, false);
            }

            return response;

        }
    }
}
