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

namespace UserServices.Application.Queries.Users
{
    public class GetAllUsersQuery : IRequest<List<UserGVM>>
    {
        public GetAllUsersQuery() { }
    }
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserGVM>>
    {
        private readonly IRepositoryService _repositoryService;
        public GetAllUsersQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<List<UserGVM>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var dbParams = new DynamicParameters();
            var results = await _repositoryService.GetJsonPathAsyncV2<List<UserGVM>>(StoredProcedures.UserSP.GET_ALL_USERS, dbParams);

            return results;

        }
    }
}
