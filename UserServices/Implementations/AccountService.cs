using Common.DTO;
using Common.Helpers;
using Common.Model.Global;
using Common.Model.Global.Users;
using Common.Model.Token;
using Common.Token;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Application.Commands.Account;
using UserServices.Application.Commands.Users;
using UserServices.Application.Queries.Users;
using UserServices.Application.Validations.Accounts;
using static Common.Constants.DataAnnotations.RequiredFields;
using static HotChocolate.ErrorCodes;

namespace UserServices
{
	public class AccountService : IAccountService
	{
		private readonly IMediator _mediator;

		private readonly ValidateInput _validateInput;
		private readonly PasswordSecurityProvider _passwordSecurityProvider;

		private readonly IJwtAuthentication _jwt;

		public AccountService(IMediator mediator, IJwtAuthentication jwt)
		{
			_mediator = mediator;
			_validateInput = new ValidateInput();
			_passwordSecurityProvider = new PasswordSecurityProvider();
			_jwt = jwt;
		}

		public async Task<LoginGVM> LoginAsync(LoginGVM postData)
		{
			var result = new LoginGVM();
			result.Email = postData.Email;


			var customValidate = new List<CutomModelErrorResponseGVM>();
			_validateInput.ValidateModelDataV3(postData);

			// throw if has errors in _validateInput
			_validateInput.ProcessCustomModelErrorResponseGVM("error");

			var getUserDetail = await _mediator.Send(new GetUserByEmailQuery(postData.Email));

			if (getUserDetail is null)
			{
				_validateInput.AddCustomModelErrorResponseGVM("email", new List<string> { "Looks like we couldn’t find that email address" });
			}

			string errLoginAttemptMsg = "You have reached your maximum login attempts. You may try again after 24 hours";

			//if (getUserDetail.LoginAttempt == 3)
			//{
			//	_validateInput.AddCustomModelErrorResponseGVM("password", new List<string> { "You have reached your maximum login attempts. You may try again after 24 hours or contact support at binsent@gmail.com" });
			//}

			// throw if has errors in _validateInput
			_validateInput.ProcessCustomModelErrorResponseGVM("error");

			// hashed password
			SecurePasswordDTO hashPassword = new SecurePasswordDTO();
			if (!string.IsNullOrEmpty(postData.Password))
			{
				hashPassword = _passwordSecurityProvider.SecurePassword(postData.Password, postData.Password, 200);
			}

			// validate password
			if (!_passwordSecurityProvider.IsValidPassword(getUserDetail.Password, postData.Password, postData.Password))
			{
				// 3 invalid attempts
				var resAddLoginAttempt = await _mediator.Send(new AddLoginAttemptCommand(getUserDetail.UserId));
				if (resAddLoginAttempt == 3)
				{
					//throw new Exception(errLoginAttemptMsg);
					_validateInput.AddCustomModelErrorResponseGVM("message", new List<string> { "You have reached your maximum login attempts. You may try again after 24 hours" });
					// force throw error.
					_validateInput.ProcessCustomModelErrorResponseGVM("error");
				}
				int attemptsLeft = 3 - resAddLoginAttempt;

				_validateInput.AddCustomModelErrorResponseGVM("message", new List<string> { $"You have {attemptsLeft} more attempts. Please make sure you enter the correct login credentials." });

				_validateInput.AddCustomModelErrorResponseGVM("password", new List<string> { "We’re having trouble recognizing your password" });

				// throw if has errors in _validateInput
				_validateInput.ProcessCustomModelErrorResponseGVM("error");
			}

			// correct password with checking if its 3attempts and expire is okay
			var resCheckLoginAttemps = await _mediator.Send(new ValidateLoginAttempt(getUserDetail.UserId));
			if (!resCheckLoginAttemps)
			{
				//throw new Exception(errLoginAttemptMsg);
				_validateInput.AddCustomModelErrorResponseGVM("message", new List<string> { "You have reached your maximum login attempts. You may try again after 24 hours" });
				// force throw error.
				_validateInput.ProcessCustomModelErrorResponseGVM("error");
			}


			var jwt = _jwt.GenerateJwtToken(getUserDetail);
			result.Token = jwt.Token;

			// insert to user token
			var resCreateToken = await _mediator.Send(new CreateUserTokenCommand(getUserDetail.UserId, jwt.Token));
			if (resCreateToken is null)
			{
				throw new Exception("The server is temporarily down, please try again later.");
			}

			return result;
		}


		public async Task<LogoutResponseGVM> LogoutAsync(string username, string token)
		{
			var result = new LogoutResponseGVM
			{
				Code = 500,
				IsSuccess = false,
				Message = "Failed to logout",
				Username = username,
				Token = token
			};

			var getUserDetail = await _mediator.Send(new GetUserByEmailQuery(username));

			if (getUserDetail is null)
			{
				_validateInput.AddCustomModelErrorResponseGVM("email", new List<string> { "Account does not exist." });
			}

			// throw if has errors in _validateInput
			_validateInput.ProcessCustomModelErrorResponseGVM("error");


			var logoutCommand = await _mediator.Send(new LogoutAccountCommand(getUserDetail.UserId, token));


			if (logoutCommand)
			{
				result.Code = 200;
				result.Message = "Success logging out";
				result.IsSuccess = true;
			}

			return result;
		}

		public async Task<bool> ValidateTokenAsync(string userId, string token)
		{
			var result = false;

			if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
			{
				var resValidateToken = await _mediator.Send(new ValidateAccountToken(userId, token));
				if (resValidateToken)
				{
					result = true;
				}
			}

			return result;
		}

		public async Task<bool> ValidateUserModuleAccessAsync(string userId, string graphqlName, string appEvent = "0")
		{
			var result = false;

			// sp validation
			if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(graphqlName))
			{
				var resValidateModuleAccess = await _mediator.Send(new ValidateUserModuleAccess(userId, graphqlName, appEvent));
				if (resValidateModuleAccess)
				{
					result = true;
				}
			}

			// validation dito haha
			if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(graphqlName))
			{
				var user = await _mediator.Send(new GetUserByIdQuery(userId));
				
				if (user != null)
				{
					if (user.RoleModuleAccess.Count() > 0)
					{
						var moduleAccess = user.RoleModuleAccess.Where(x => x.IsAccess == true && x.GraphQLName == graphqlName );

						if (moduleAccess != null)
						{
							if (appEvent != "0")
							{
								if (moduleAccess.First().AccessScript.Contains(appEvent))
								{
									result = true;
								} else
								{
									result = false;
								}
							}
							else
							{
								result = true;
							}
						}
					}
				}
			}

			return result;
		}


		public async Task<bool> ValidateTokenAndModuleAccessAsync(string userId, string token, string graphqlName, string appEvent = "0")
		{
			var result = false;

			if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(graphqlName))
			{
				var resValidateToken = await _mediator.Send(new ValidateAccountToken(userId, token));
				if (resValidateToken)
				{
					var resValidateModuleAccess = await _mediator.Send(new ValidateUserModuleAccess(userId, graphqlName, appEvent));
					if (resValidateModuleAccess)
					{
						result = true;
					}
				}
			}

			return result;
		}
	}
}
