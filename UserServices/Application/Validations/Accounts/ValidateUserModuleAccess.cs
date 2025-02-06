using Common.Constants;
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
	public class ValidateUserModuleAccess : IRequest<bool>
	{
		public string UserId { get; set; }
		public string GraphQLName { get; set; }
		public string AppEvent { get; set; } = "0";
		public ValidateUserModuleAccess(string userId, string gqlName, string appEvent = "0")
		{
			UserId = userId;
			GraphQLName = gqlName;
			AppEvent = appEvent;
		}
	}

	public class ValidateUserModuleAccessHandler : IRequestHandler<ValidateUserModuleAccess, bool>
	{
		private readonly IRepositoryService _repositoryService;
		public ValidateUserModuleAccessHandler(IRepositoryService repository)
		{
			_repositoryService = repository;
		}
		public async Task<bool> Handle(ValidateUserModuleAccess request, CancellationToken cancellationToken)
		{
			var dbParams = new DynamicParameters();
			dbParams.Add("command", "module");
			dbParams.Add("userId", request.UserId);
			dbParams.Add("graphqlName", request.GraphQLName);
			dbParams.Add("appEvent", request.AppEvent);

			var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_ACCOUNT, dbParams);

			return results;
		}
	}
}
