using Common.Constants;
using Common.Helpers;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
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
using UserServices.Application.Commands.Users;

namespace UserServices.Application.Commands.Users
{
    public class CreateUserCommand : IRequest<CustomModelResponse>
    {
        public UserGVM User { get; set; }

        public CreateUserCommand(UserGVM postData)
        {
            User = postData;
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CustomModelResponse>
    {
        private readonly IRepositoryService _repositoryService;
        public CreateUserCommandHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<CustomModelResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var response = new CustomModelResponse(ApiResponseMessage.Exceptions.INTERNAL_SERVER_MESSAGE, 500, false);
            try
            {
                var jsonSettings = new JsonSerializerSettings()
                { ContractResolver = new IgnorePropertiesResolver(new[] { "Merchants", "RoleModuleAccess", "RoleName" }) };
                var userJsonRes = JsonConvert.SerializeObject(request.User, jsonSettings);
                

                var dbParams = new DynamicParameters();
                dbParams.Add("userData", userJsonRes);

                var results = await _repositoryService.ExecuteWithReturnAsync<string>(StoredProcedures.UserSP.CREATE_USER, dbParams);

                response = new CustomModelResponse(ApiResponseMessage.SUCCESS, 200, true, results.ToString());
            }
            catch (Exception ex)
            {
                var errMsg = (ex.InnerException is null) ? ex.Message : ex.InnerException.Message;
                response = new CustomModelResponse(errMsg, 500, false);
            }

            return response;

        }
    }
}
