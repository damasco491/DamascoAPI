using Common.Constants;
using Dapper;
using Datalayer;
using MediatR;
using System.Security.Principal;

namespace UserServices.Application.Validations.Users
{
    public class ValidateUserEmail : IRequest<bool>
    {
        public string Email { get; set; }
        public ValidateUserEmail(string email)
        {
            Email = email;
        }
    }

    public class ValidateUserEmailHandler : IRequestHandler<ValidateUserEmail, bool>
    {
        private readonly IRepositoryService _repositoryService;
        public ValidateUserEmailHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<bool> Handle(ValidateUserEmail request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            dbParams.Add("command", "email");
            dbParams.Add("Email", request.Email);

            var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_USER, dbParams);

            return results;
        }
    }
}
