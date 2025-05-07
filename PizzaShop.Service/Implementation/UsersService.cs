using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.Service.Implementation;

public class UsersService : IUsersService
{
    private readonly IUserRepository _user;
    private readonly IAccountRepository _account;
    private readonly IRoleRepository _role;

    public UsersService(IUserRepository user, IAccountRepository account,IRoleRepository role)
    {
        _user = user;
        _account = account;
        _role = role;
    }

    public async Task<List<UsersViewModel>> GetUsers(string searchString, int pageIndex, int pageSize, string? sortBy, string? sortType)
    {
        // here we return users which have isDeleted = false
        var userList = await _user.GetUsers(searchString, pageIndex, pageSize, sortBy, sortType);
        return userList;
    }

    public async Task<int> GetUsersCount(string searchString)
    {
        return await _user.getUsersCount(searchString);
    }

    public async Task<bool> CreateUser(CreateUserViewModel model, string userId)
    {
        string ProfileImage = null;
        if (model.ProfileimagePath != null && model.ProfileimagePath.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ProfileImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileimagePath.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfileimagePath.CopyTo(stream);
            }
            ProfileImage = "/ProfileImages/" + filename;
        }
        if (ProfileImage != null)
            model.Profileimage = ProfileImage;

        var hashPassword = HashPassword(model.Password);
        model.Password = hashPassword;
        await _user.CreateUser(model, userId);
        await _account.CreateAccount(model,userId);

        return true;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _user.GetUserByEmail(email);
        return user;
    }
    
    public async Task<UpdateUserViewModel> GetUpdateUserDetail(int id)
    {
        UpdateUserViewModel updateUser = await _user.GetUpdateUserDetail(id);
        updateUser.Roles = await _role.GetAllRoles();
        return updateUser;
    }

    public async Task UpdateUser(UpdateUserViewModel model,string userId)
    {
        string ProfileImage = null;
        if (model.ProfileimagePath != null && model.ProfileimagePath.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ProfileImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileimagePath.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfileimagePath.CopyTo(stream);
            }
            ProfileImage = "/ProfileImages/" + filename;
        }
        if (ProfileImage != null)
            model.Profileimage = ProfileImage;
            
        // var user = _user.GetUserByEmail(model.Email);
        await _user.UpdateUser(model,userId);
        await _account.updateAccount(model,userId);
    }

    public async Task<User> GetUserById(int id)
    {
        return await _user.GetUserById(id);
    }
    
    public async Task DeleteUser(int id, string userId)
    {   
        // here i passed user id so we can put id or role in the place of updated By
        await _user.DeleteUser(id, userId);
        _account.DeleteAccount(id, userId);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _role.GetAllRoles();
    }
}
