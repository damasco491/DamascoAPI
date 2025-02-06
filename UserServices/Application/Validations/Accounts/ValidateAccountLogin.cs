using Common.Constants;
using Common.Model.Global.Users;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Validations.Users;

namespace UserServices.Application.Validations.Accounts
{
    public class ValidateAccountLogin : IRequest<bool>
    {
        public LoginGVM LoginDetail { get; set; }
        public ValidateAccountLogin(LoginGVM postData)
        {
            LoginDetail = postData;
        }
    }

    public class ValidateAccountLoginHandler : IRequestHandler<ValidateAccountLogin, bool>
    {
        private readonly IRepositoryService _repositoryService;
        public ValidateAccountLoginHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<bool> Handle(ValidateAccountLogin request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            dbParams.Add("command", "email");
            dbParams.Add("Email", request.LoginDetail.Email);

            var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_USER, dbParams);

            return results;
        }
    }
}
