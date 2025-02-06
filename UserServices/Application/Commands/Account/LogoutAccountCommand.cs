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

namespace UserServices.Application.Commands.Account
{
	public class LogoutAccountCommand : IRequest<bool>
	{
		public string UserId { get; set; }
		public string Token { get; set; }
		public LogoutAccountCommand(string userId, string token)
		{
			UserId = userId;
			Token = token;
		}
	}

	public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand, bool>
	{
		private readonly IRepositoryService _repositoryService;
		public LogoutAccountCommandHandler(IRepositoryService repository)
		{
			_repositoryService = repository;
		}
		public async Task<bool> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
		{
			bool isSuccess = false;
			try
			{


				var dbParams = new DynamicParameters();
				dbParams.Add("userId", request.UserId);
				dbParams.Add("token", request.Token);

				var results = await _repositoryService.ExecuteWithReturnAsync<int>(StoredProcedures.AccountSP.LOGOUT_ACCOUNT, dbParams);

				isSuccess = (results == 1) ;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

			return isSuccess;

		}
	}
}
