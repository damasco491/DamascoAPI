using Common.Constants;
using Common.Helpers;
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

namespace UserServices.Application.Commands.Users
{
    public class CreateUserTokenCommand : IRequest<CustomModelResponse>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public CreateUserTokenCommand(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }

    public class CreateUserTokenCommandHandler : IRequestHandler<CreateUserTokenCommand, CustomModelResponse>
    {
        private readonly IRepositoryService _repositoryService;
        public CreateUserTokenCommandHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<CustomModelResponse> Handle(CreateUserTokenCommand request, CancellationToken cancellationToken)
        {
            var response = new CustomModelResponse(ApiResponseMessage.Exceptions.INTERNAL_SERVER_MESSAGE, 500, false);
            try
            {
                

                var dbParams = new DynamicParameters();
                dbParams.Add("userId", request.UserId);
                dbParams.Add("token", request.Token);

                var results = await _repositoryService.ExecuteWithReturnAsync<string>(StoredProcedures.UserSP.CREATE_USER_TOKEN, dbParams);

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
