using PizzaShop.DataAccess.Data;
using PizzaShop.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUsersService
{
     public Task<List<UsersViewModel>> GetUsers(string searchString, int pageIndex, int pageSize, string? sortBy, string? sortType);

    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserById(int id);
    public Task<bool> CreateUser(CreateUserViewModel model, string email);
    public Task<int> GetUsersCount(string searchString);
    public Task<UpdateUserViewModel> GetUpdateUserDetail(int id);
    public Task UpdateUser(UpdateUserViewModel model,string userId);
    public Task DeleteUser(int id, string userId);
    public Task<List<Role>> GetAllRoles();
}
