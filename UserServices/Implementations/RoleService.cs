using Common.Helpers;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using HotChocolate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Commands.Roles;
using UserServices.Application.Queries.Roles;
using static Common.Constants.StoredProcedures;

namespace UserServices
{
    public class RoleService : IRoleService
    {
        private readonly IMediator _mediator;

        private readonly ValidateInput _validateInput;
        public RoleService(IMediator mediator)
        {
            _mediator = mediator;
            _validateInput = new ValidateInput();
        }
        public async Task<List<RoleGVM>> GetRolesAsync()
        {
            var result = await _mediator.Send(new GetRolesQuery());

            if (result is null)
            {
                result = new List<RoleGVM>();
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task<RoleGVM> GetRoleByIdAsync(string roleId)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(roleId));
            if (result is null)
            {
                result = new RoleGVM();
            }

            return result;
        }

        public async Task<RoleGVM> CreateRoleAsync(RoleGVM role)
        {
            // setting default values to required field.
            role.RoleId = "new";
            var result = new RoleGVM();

            _validateInput.ValidateModelDataV3(role);

            if (_validateInput.GraphQLError.Count() > 0)
            {
                throw new GraphQLException(_validateInput.GraphQLError);
            }
            

            var createRoleResponse = await _mediator.Send(new CreateRoleCommand(role));

            if (createRoleResponse is null)
            {
                throw new Exception("The server is temporarily down, please try again later.");
            }

            if (!createRoleResponse.IsSuccessful)
            {
                //throw new Exception(createRoleResponse.Errors.GetType().GetProperty("message").GetValue(createRoleResponse.Errors, null).ToString());
                throw new Exception(createRoleResponse.Message);
            } else
            {
                result = await _mediator.Send(new GetRoleByIdQuery(createRoleResponse.CreatedIdResult));
            }

            return result;

        }
        public async Task<RoleGVM> UpdateRoleAsync(RoleGVM role)
        {
            var result = new RoleGVM();

			_validateInput.ValidateModelDataV3(role);

			if (_validateInput.GraphQLError.Count() > 0)
			{
				throw new GraphQLException(_validateInput.GraphQLError);
			}

			var createRoleResponse = await _mediator.Send(new UpdateRoleCommand(role));
            if (createRoleResponse is null)
            {
                throw new Exception("The server is temporarily down, please try again later.");
            }

            if (!createRoleResponse.IsSuccessful)
            {
                throw new Exception(createRoleResponse.Message);
            }
            else
            {
                result = await _mediator.Send(new GetRoleByIdQuery(role.RoleId));
            }

            return result;
        }
    }
}
