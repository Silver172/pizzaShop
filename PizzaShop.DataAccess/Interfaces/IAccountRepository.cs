using PizzaShop.DataAccess.Data;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IAccountRepository
{
    public Task<Account> GetAccountByEmail(string email);
    public Task<Account> UpdatePassword(string email, string password);
    public Task CreateAccount(CreateUserViewModel model, string userId);
    public Task updateAccount(UpdateUserViewModel model,string userId);
    public Task DeleteAccount(int id, string userId);
    public Task MarkUserFirstLogin(string email);
}
