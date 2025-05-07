using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class AccountRepository : IAccountRepository
{
    private readonly PizzashopContext _context;
    private readonly IRoleRepository _role;
    private readonly IUserRepository _user;
    public AccountRepository(PizzashopContext context, IRoleRepository role,IUserRepository user)
    {
        _context = context;
        _role = role;
        _user = user;
    }

    public async Task<Account> GetAccountByEmail(string email)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        return account;
    }

    public async Task<Account> UpdatePassword(string email, string password)
    {
        User user = await _user.GetUserByEmail(email);
        Account? account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        account.Password = password;
        account.Updateddate = DateTime.Now;
        account.Updatedby = user.Id.ToString();
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task CreateAccount(CreateUserViewModel model, string userId)
    {
        var newAccount = new Account
        {
            Email = model.Email,
            Roleid = _context.Roles.FirstOrDefault(r => r.Id == int.Parse(model.Role)).Id,
            Password = model.Password,
            Createdby = userId,
            Createddate = DateTime.Now
        };
        await _context.Accounts.AddAsync(newAccount);
        await _context.SaveChangesAsync();
    }

    public async Task updateAccount(UpdateUserViewModel model,string userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.Id);

        if (account != null)
        {
            account.Roleid = int.Parse(model.Role);
            account.Updatedby = userId;
            account.Updateddate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
    public async Task DeleteAccount(int id, string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == user.Email);

        account.Isdeleted = true;
        account.Updatedby = userId;
        await _context.SaveChangesAsync();
    }

     public async Task MarkUserFirstLogin(string email)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        account.Isfirstlogin = true;
    }
}
