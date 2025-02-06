using Common.Constants;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using Common.ViewModels;
using Dapper;
using Datalayer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Queries.Roles;

namespace UserServices.Application.Queries.Users
{
    public class GetUserByIdQuery : IRequest<UserGVM>
    {
        public string UserId { get; set; }
        public GetUserByIdQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserGVM>
    {
        private readonly IRepositoryService _repositoryService;
        public GetUserByIdQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<UserGVM> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {

            var results = new UserGVM();
            try
            {
                var dbParams = new DynamicParameters();
                dbParams.Add("userId", request.UserId);
                results = await _repositoryService.GetJsonPathAsyncV2<UserGVM>(StoredProcedures.UserSP.GET_USER_BY_ID, dbParams);

                var res = results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return results;
        }
    }
}
