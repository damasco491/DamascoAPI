using Common.Constants;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Application.Validations.Users
{
	public class ValidateUserRoleId : IRequest<bool>
	{
		public string RoleId { get; set; }
		public ValidateUserRoleId(string roleId)
		{
			RoleId = roleId;
		}
	}

	public class ValidateUserRoleIdHandler : IRequestHandler<ValidateUserRoleId, bool>
	{
		private readonly IRepositoryService _repositoryService;
		public ValidateUserRoleIdHandler(IRepositoryService repository)
		{
			_repositoryService = repository;
		}
		public async Task<bool> Handle(ValidateUserRoleId request, CancellationToken cancellationToken)
		{
			var dbParams = new DynamicParameters();
			dbParams.Add("command", "roleId");
			dbParams.Add("roleId", request.RoleId);

			var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_USER, dbParams);

			return results;
		}
	}
}
