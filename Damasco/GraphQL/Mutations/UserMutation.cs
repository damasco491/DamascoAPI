using API.GraphQL;
using Common.Constants;
using Common.Model.Global.Email;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using HotChocolate.Authorization;
using LoggerServices.Interfaces;
using Newtonsoft.Json;
//using NotificationServices;
using System.Data;
using System.Security.Claims;
using UserServices;

namespace Damasco.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
		private readonly ValidateModuleHeader _validateModuleHeader;
		private readonly ILoggerService _loggerService;
		public UserMutation(ValidateModuleHeader validateModuleHeader, ILoggerService loggerService)
		{
			_validateModuleHeader = validateModuleHeader;
            _loggerService = loggerService;
		}

		[GraphQLName("createUser")]
		//[Authorize]
		//public async Task<UserGVM> CreateUserAsync(HttpContext context, ClaimsPrincipal claimsPrincipal, [Service] IUserService userService, [Service] IEmailService emailService, UserGVM user)
		public async Task<UserGVM> CreateUserAsync(HttpContext context, ClaimsPrincipal claimsPrincipal, [Service] IUserService userService, UserGVM user)
        {
            var result = new UserGVM();

            try
            {

				// validate header  PS: userName = email
				//await _validateModuleHeader.ValidateTokenAndAccess(context, claimsPrincipal, "users", "2");
				// end validate header

				string notHashPassword = user.Password;
				user.NotHashPassword = notHashPassword;
                result = await userService.CreateUserAsync(user);

                var userData = new UserGVM();

                userData = result;
                userData.Password = user.NotHashPassword;
				

                var request = context.Request;
                string baseURL = $@"{request.Scheme}://{request.Host}/";
                userData.AppUrl = $@"{baseURL}account/auth";
                //var emailRes = await emailService.SendEmailV2Async(user);
                //EmailTemplate emailTemplateRes = await emailService.GenerateEmailTemplateAsync(userData, 1);
                //if(emailTemplateRes != null)
                //{
                //    var emailRes = await emailService.SendEmailAsync(emailTemplateRes.EmailSubject, user.Email, emailTemplateRes.EmailBody);
                //    var x = emailRes;
                //}

                result.NotHashPassword = "";
                result.AppUrl = "";

                //await _loggerService.LogInfo(LogType.SUCCESS, this.ToString(), LogMessage.SUCCESS_MESSAGE, "");
			}
			catch (GraphQLException ex)
			{
				//await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", JsonConvert.SerializeObject(user));

				throw;
			}

			catch (Exception ex)
			{
				var error = new Error(ex.Message, "500");
				//await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", JsonConvert.SerializeObject(user));
				throw new GraphQLException(error);
			}

			return result;
        }
    }
}
