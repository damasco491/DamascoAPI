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

namespace UserServices.Application.Validations.Accounts
{
	public class ValidateLoginAttempt : IRequest<bool>
	{
		public string UserId { get; set; }
		public ValidateLoginAttempt(string userId)
		{
			UserId = userId;
		}
	}

	public class ValidateLoginAttemptHandler : IRequestHandler<ValidateLoginAttempt, bool>
	{
		private readonly IRepositoryService _repositoryService;
		public ValidateLoginAttemptHandler(IRepositoryService repository)
		{
			_repositoryService = repository;
		}
		public async Task<bool> Handle(ValidateLoginAttempt request, CancellationToken cancellationToken)
		{
			var dbParams = new DynamicParameters();
			dbParams.Add("command", "loginAttempts");
			dbParams.Add("userId", request.UserId);

			var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_ACCOUNT, dbParams);

			return results;
		}
	}
}
