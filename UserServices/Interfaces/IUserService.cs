using Common.Model.Global.Users;

namespace UserServices
{
    public interface IUserService
    {
        public Task<List<UserGVM>> GetUsersAsync();
        public Task<UserGVM> GetUserByEmailAsync(string email);
        public Task<UserGVM> CreateUserAsync(UserGVM user);
    }
}
