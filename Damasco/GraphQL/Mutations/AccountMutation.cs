using Common.Constants;
using Common.Model.Global.Email;
using Common.Model.Global.Users;
using LoggerServices.Implementations;
using LoggerServices.Interfaces;
using Newtonsoft.Json;
//using NotificationServices;
using System.Security.Claims;
using UserServices;
using Microsoft.AspNetCore.SignalR;
//using SignalR.Hubs;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.SignalR.Client;


namespace API.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class AccountMutation
    {
		private readonly ValidateModuleHeader _validateModuleHeader;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public AccountMutation( ValidateModuleHeader validateModuleHeader,  IConfiguration configuration)
		{
			_validateModuleHeader = validateModuleHeader;
            _configuration = configuration;
        }

		[GraphQLName("login")]
        //public async Task<LoginGVM> AccountLoginAsync([Service] ILoggerService _loggerService, ClaimsPrincipal claimsPrincipal, [Service] IAccountService accountService, [Service] IEmailService emailService, LoginGVM loginDetails)
        public async Task<LoginGVM> AccountLoginAsync([Service] ILoggerService _loggerService, ClaimsPrincipal claimsPrincipal, [Service] IAccountService accountService,  LoginGVM loginDetails)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = claimsPrincipal.Identity.Name;
            var result = new LoginGVM();

            try
            {
                string notHashPassword = loginDetails.Password;

                var loginRes = await accountService.LoginAsync(loginDetails);
                
                result = loginRes;
                // Notify the SignalR hub
                // Send login result to SignalR service
                //var connection = new HubConnectionBuilder()
                //.WithUrl(_configuration["SignalR:AuthHub"], options =>
                //{
                //    options.AccessTokenProvider = () => Task.FromResult(result.Token);
                //})
                //.Build();

                //await connection.StartAsync();
                //await connection.InvokeAsync("ReceiveLoginResult", result);
                //await connection.StopAsync();


            }
            catch (GraphQLException ex)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", JsonConvert.SerializeObject(loginDetails));

				throw;
			}

			catch (Exception ex)
			{
				var error = new Error(ex.Message, "500");
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", JsonConvert.SerializeObject(loginDetails));
				throw new GraphQLException(error);
			}

			return result;
        }

		[GraphQLName("logout")]
		public async Task<LogoutResponseGVM> AccountLogoutAsync([Service] ILoggerService _loggerService, ClaimsPrincipal claimsPrincipal, HttpContext context, [Service] IAccountService accountService)
		{
			var result = new LogoutResponseGVM();

			try
			{

				if (claimsPrincipal is null)
				{
					await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE}", "");
					throw new GraphQLException("Unauthorized");
				}

				var claimEmail = claimsPrincipal.FindFirst(ClaimTypes.Email);
				// Access headers from the HTTP request
				var headers = context.Request.Headers;

				// Retrieve JWT token from the "Authorization" header
				var jwtToken = String.IsNullOrEmpty(headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault()) ? "" : headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault();



				var loginRes = await accountService.LogoutAsync(claimEmail.Value, jwtToken);

				result = loginRes;

			}
			catch (GraphQLException ex)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", "");

				throw;
			}

			catch (Exception ex)
			{
				var error = new Error(ex.Message, "500");
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", "");
				throw new GraphQLException(error);
			}

			return result;
		}
	}
}
