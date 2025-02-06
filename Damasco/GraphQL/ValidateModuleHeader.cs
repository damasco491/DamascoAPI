using Common.Constants;
using LoggerServices.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UserServices;

namespace API.GraphQL
{
	public class ValidateModuleHeader
	{
		private readonly IAccountService accountService;
		private readonly ILoggerService _loggerService;
		public ValidateModuleHeader(IAccountService accountSvc, ILoggerService loggerService)
		{
			accountService = accountSvc;
			_loggerService = loggerService;
		}

		public async void ValidateHeader(string userId, string graphqlName, string appEvent = "0")
		{
			bool hasAccess = await accountService.ValidateUserModuleAccessAsync(userId, graphqlName, appEvent);

			if (!hasAccess)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE} : ", "");
				throw new GraphQLException("Unauthorized");
			}
		}

		public async Task ValidateTokenAndAccess(HttpContext context, ClaimsPrincipal claimsPrincipal, string graphqlName, string appEvent = "0")
		{

			if (context is null)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE} : Context is Null", graphqlName);
				throw new GraphQLException("Unauthorized");
			}

			if (claimsPrincipal is null)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE} : Claims is Null", graphqlName);
				throw new GraphQLException("Unauthorized");
			}


			// validate header PS: userName = email
			var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
			// Access headers from the HTTP request
			var headers = context.Request.Headers;

			// Retrieve JWT token from the "Authorization" header
			var jwtToken = string.IsNullOrEmpty(headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault()) ? "" : headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault();

			if (!string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(userId))
			{
				bool hasAccess = await accountService.ValidateTokenAndModuleAccessAsync(userId, jwtToken, graphqlName, appEvent);
				if (!hasAccess)
				{
					await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE} : User does'nt have access to this app event.", graphqlName);
					throw new GraphQLException("Unauthorized");
				}
			}
			else
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.UNAUTHORIZED_MESSAGE} : Module access denied", graphqlName);
				throw new GraphQLException("Unauthorized");
			}
		}

		

	}
}
