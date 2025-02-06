using Common.Constants;
using Common.ViewModels;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Commands.Users;

namespace UserServices.Application.Commands.Account
{
    public class AddLoginAttemptCommand : IRequest<int>
    {
        public string UserId { get; set; } 
        public AddLoginAttemptCommand(string userId) { 
            UserId = userId;
        }
    }

    public class AddLoginAttemptCommandHandler : IRequestHandler<AddLoginAttemptCommand, int>
    {
        private readonly IRepositoryService _repositoryService;
        public AddLoginAttemptCommandHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<int> Handle(AddLoginAttemptCommand request, CancellationToken cancellationToken)
        {
            int loginAttempts = 0;
            try
            {


                var dbParams = new DynamicParameters();
                dbParams.Add("userId", request.UserId);

                var results = await _repositoryService.ExecuteWithReturnAsync<int>(StoredProcedures.UserSP.ADD_LOGIN_ATTEMPT, dbParams);

                loginAttempts = results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return loginAttempts;

        }
    }

}
