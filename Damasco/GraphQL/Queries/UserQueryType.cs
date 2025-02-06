using API.GraphQL;
using Common.Constants;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using HotChocolate.Authorization;
using LoggerServices.Interfaces;
using System.Security.Claims;
using UserServices;

namespace Damasco.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class UserQueryType
    {
		private readonly ValidateModuleHeader _validateModuleHeader;
		private readonly ILoggerService _loggerService;
		public UserQueryType(ValidateModuleHeader validateModuleHeader, ILoggerService loggerService)
		{
			_validateModuleHeader = validateModuleHeader;
			_loggerService = loggerService;
		}

		[GraphQLName("users")]
		[Authorize]
		//[UseOffsetPaging(IncludeTotalCount = true)] // remove, client side.
		[UseFiltering]
        [UseSorting]
        public async Task<IQueryable<UserGVM>> GetUsersAsync(HttpContext context, ClaimsPrincipal claimsPrincipal, [Service] IUserService userService)
        {

            var results = new List<UserGVM>();
            try
            {
				// validate header  PS: userName = email
				await _validateModuleHeader.ValidateTokenAndAccess(context, claimsPrincipal, "users", "0");
				// end validate header

				results = await userService.GetUsersAsync();
				await _loggerService.LogInfo(LogType.SUCCESS, this.ToString(), LogMessage.SUCCESS_MESSAGE, "");
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

			return results.AsQueryable();
        }

		[GraphQLName("userProfile")]
		[Authorize]
		public async Task<UserGVM> GetUserProfileAsync(HttpContext context, ClaimsPrincipal claimsPrincipal, [Service] IUserService userService, string email)
		{

			var results = new UserGVM();
			try
			{
				if (context is null)
				{
					throw new GraphQLException("Unauthorized");
				}

				if (claimsPrincipal is null)
				{
					throw new GraphQLException("Unauthorized");
				}

				var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
				var userEmail = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

				if (email == userEmail)
				{
					results = await userService.GetUserByEmailAsync(email);
				} else
				{
					throw new GraphQLException("Unauthorized");
				}


				await _loggerService.LogInfo(LogType.SUCCESS, this.ToString(), LogMessage.SUCCESS_MESSAGE, "");
			}
			catch (GraphQLException ex)
			{
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", email);
				throw;
			}

			catch (Exception ex)
			{
				var error = new Error(ex.Message, "500");
				await _loggerService.LogInfo(LogType.ERROR, this.ToString(), $"{LogMessage.EXCEPTION_ERROR_MESSAGE} : {ex.Message}", email);
				throw new GraphQLException(error);
			}

			return results;
		}
	}
}
