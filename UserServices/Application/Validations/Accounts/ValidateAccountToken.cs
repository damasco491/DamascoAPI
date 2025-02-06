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
	public class ValidateAccountToken : IRequest<bool>
	{
		public string UserId { get; set; }
		public string Token { get; set; }
		public ValidateAccountToken(string userId, string token)
		{
			UserId = userId;
			Token = token;
		}
	}

	public class ValidateAccountTokenHandler : IRequestHandler<ValidateAccountToken, bool>
	{
		private readonly IRepositoryService _repositoryService;
		public ValidateAccountTokenHandler(IRepositoryService repository)
		{
			_repositoryService = repository;
		}
		public async Task<bool> Handle(ValidateAccountToken request, CancellationToken cancellationToken)
		{
			var dbParams = new DynamicParameters();
			dbParams.Add("command", "token");
			dbParams.Add("userId", request.UserId);
			dbParams.Add("token", request.Token);

			var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_ACCOUNT, dbParams);

			return results;
		}
	}
}
