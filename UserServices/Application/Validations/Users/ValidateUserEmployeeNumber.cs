using Common.Constants;
using Dapper;
using Datalayer;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Application.Validations.Users
{
    public class ValidateUserEmployeeNumber : IRequest<bool>
    {
        public string EmployeeNumber { get; set; }
        public List<string> MerchantIds { get; set; }
        public ValidateUserEmployeeNumber(string employeeNumber, List<string> merchantIds)
        {
            EmployeeNumber = employeeNumber;
            MerchantIds = merchantIds;
        }
    }

    public class ValidateUserEmployeeNumberHandler : IRequestHandler<ValidateUserEmployeeNumber, bool>
    {
        private readonly IRepositoryService _repositoryService;
        public ValidateUserEmployeeNumberHandler(IRepositoryService repository)
        {
            _repositoryService = repository;
        }
        public async Task<bool> Handle(ValidateUserEmployeeNumber request, CancellationToken cancellationToken)
        {
            var merchantIdsJsonRes = JsonConvert.SerializeObject(request.MerchantIds);

            var dbParams = new DynamicParameters();
            dbParams.Add("command", "employeeNumber");
            dbParams.Add("employeeNumber", request.EmployeeNumber);
            dbParams.Add("merchantIds", merchantIdsJsonRes);

            var results = _repositoryService.Get<bool>(StoredProcedures.UserSP.VALIDATE_USER, dbParams);

            return results;
        }
    }
}
