using Common.Model.Global.Users;
using Common.Model.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices
{
    public interface IAccountService
    {
        public Task<LoginGVM> LoginAsync(LoginGVM postData);

        public Task<LogoutResponseGVM> LogoutAsync(string username, string token);

        public Task<bool> ValidateTokenAsync(string userId, string token);

		// appEvent = 0(default/list), 1(view detail), 2 (add), 3(edit), 4(delete)
		public Task<bool> ValidateUserModuleAccessAsync(string userId, string graphqlName, string appEvent = "0");

        public Task<bool> ValidateTokenAndModuleAccessAsync(string userId, string token, string graphqlName, string appEvent = "0");
	}
}
