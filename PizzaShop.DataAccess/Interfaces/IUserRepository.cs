using PizzaShop.DataAccess.Data;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IUserRepository
{
    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserById(int id);
    public Task<User> UpdateUserProfileByEmail(ProfileViewModel model, string userId);
    public Task<List<UsersViewModel>> GetUsers(string searchString, int pageIndex, int pageSize, string? sortBy, string? sortType);
    public Task<int> getUsersCount(string searchString);
    public Task DeleteUser(int id, string userId);
    public Task<UpdateUserViewModel> GetUpdateUserDetail(int id);
    public Task UpdateUser(UpdateUserViewModel model, string userId);
    public Task CreateUser(CreateUserViewModel model, string userId);
}
