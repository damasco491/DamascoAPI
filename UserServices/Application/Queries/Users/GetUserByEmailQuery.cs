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
    public class GetUserByEmailQuery : IRequest<UserGVM>
    {
        public string Email { get; set; }
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserGVM>
    {
        private readonly IRepositoryService _repositoryService;
        public GetUserByEmailQueryHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<UserGVM> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {

            var results = new UserGVM();
            try
            {
                var dbParams = new DynamicParameters();
                dbParams.Add("email", request.Email);
                results = await _repositoryService.GetJsonPathAsyncV2<UserGVM>(StoredProcedures.UserSP.GET_USER_BY_EMAIL, dbParams);
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
