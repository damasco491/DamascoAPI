using Common.DTO;
using Common.Helpers;
using Common.Model.Global;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using HotChocolate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Commands.Users;
using UserServices.Application.Queries.Roles;
using UserServices.Application.Queries.Users;
using UserServices.Application.Validations.Users;

namespace UserServices
{
    public class UserService : IUserService
    {
        private readonly IMediator _mediator;

        private readonly ValidateInput _validateInput;
        private readonly PasswordSecurityProvider _passwordSecurityProvider;
        public UserService(IMediator mediator)
        {
            _mediator = mediator;
            _validateInput = new ValidateInput();
            _passwordSecurityProvider = new PasswordSecurityProvider();
        }
        public async Task<List<UserGVM>> GetUsersAsync()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());

            if (result is null)
            {
                result = new List<UserGVM>();
            }


            return result;
        }

		public async Task<UserGVM> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new GraphQLException("Email is required");
            }


			var result = await _mediator.Send(new GetUserByEmailQuery(email));

			result ??= new UserGVM();


			return result;
		}
		public async Task<UserGVM> CreateUserAsync(UserGVM user)
        {
            // setting default values to required field.
            user.UserId = "new";
            user.Username = user.Email;
            var result = new UserGVM();

            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = StringHelper.GenerateRandomString(15);
                user.NotHashPassword = user.Password;
            }

            //// explode merchantids
            //List<string> merchantIds = new List<string>();
            //if (!string.IsNullOrEmpty(user.MerchantIds) && user.MerchantIds.Contains(','))
            //{
            //    merchantIds = user.MerchantIds.Split(',').Select(p => p.Trim()).ToList();
            //}
            //else
            //{
            //    merchantIds.Add(user.MerchantIds);
            //}
            //// end explode

            var customValidate = new List<CutomModelErrorResponseGVM>();
            _validateInput.ValidateModelDataV3(user);

            if (_validateInput.GraphQLError.Count() > 0)
            {
                throw new GraphQLException(_validateInput.GraphQLError);
            }

            var isEmailValid = await _mediator.Send<bool>(new ValidateUserEmail(user.Email));
            if (!isEmailValid)
            {
                _validateInput.AddCustomModelErrorResponseGVM("email", new List<string> { "Email Address already taken." });
            }

         

			var isRoleIdValid = await _mediator.Send<bool>(new ValidateUserRoleId(user.RoleId));
			if (!isRoleIdValid)
			{
				_validateInput.AddCustomModelErrorResponseGVM("role", new List<string> { "Role does not exists." });
			}

			// throw if has errors in _validateInput
			_validateInput.ProcessCustomModelErrorResponseGVM("error");

            // hashed password
            if (!string.IsNullOrEmpty(user.Password))
            {
                var hashPassword = _passwordSecurityProvider.SecurePassword(user.Password, user.Password, 200);
                user.Password = hashPassword.HashPassword;
            } 
            //else
            //{
            //    //_validateInput.AddCutomModelErrorResponseGVM(ref customValidate, "password", new List<string> { "Your credentials are wrong. Please try again." });
            //}
            // end hash

            var createUserRes = await _mediator.Send(new CreateUserCommand(user));
            if (createUserRes is null)
            {
                throw new Exception("The server is temporarily down, please try again later.");
            }

            if (!createUserRes.IsSuccessful)
            {
                throw new Exception(createUserRes.Message);
            }
            else
            {
                
                result = await _mediator.Send(new GetUserByIdQuery(createUserRes.CreatedIdResult));
            }


            return result;
        }
    }
}
