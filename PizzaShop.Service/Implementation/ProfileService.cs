using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.Service.Implementation;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _user;
    private readonly ICountryRepository _country;
    private readonly IRoleRepository _role;
    private readonly IAccountRepository _account;

    public ProfileService(IUserRepository user, ICountryRepository country, IRoleRepository role, IAccountRepository account)
    {
        _user = user;
        _country = country;
        _role = role;
        _account = account;
    }

    public async Task<ProfileViewModel> GetUserProfile(string? email)
    {
        var user = await _user.GetUserByEmail(email);
        var role = await _role.GetRoleById(user.Roleid);

        var profileModel = new ProfileViewModel
        {
            FirstName = user.Firstname,
            LastName = user.Lastname,
            UserName = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Country = user.Country,
            State = user.State,
            City = user.City,
            Address = user.Address,
            ZipCode = user.Zipcode,
            Role = role,
            ProfileImagePath = user.Profileimage
        };
        return profileModel;
    }

    public async Task<bool> UpdateProfileByEmail(ProfileViewModel model, string userId)
    {
        string ProfileImagePath = null;
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ProfileImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfileImage.CopyTo(stream);
            }
            ProfileImagePath = "/ProfileImages/" + filename;
        }
        if (ProfileImagePath != null)
            model.ProfileImagePath = ProfileImagePath;
        var updatedUser = await _user.UpdateUserProfileByEmail(model, userId);
        if (updatedUser != null)
            return true;
        return false;
    }

    public async Task<string> ChangePassword(ChangePasswordViewModel model, string email)
    {
        var account = await _account.GetAccountByEmail(email);
        if (account == null)
            return "user not found";
        var isOldPasswordValid = VerifyPassword(model.CurrentPassword, account.Password);
        if (!isOldPasswordValid)
            return "Current Password Is Incorrect";
        var hashpassword = HashPassword(model.NewPassword);

        if(HashPassword(model.NewPassword) == account.Password)
            return "New password must be different from current password";

        var passwordUpdate = await _account.UpdatePassword(email, hashpassword);
        if (passwordUpdate.Password == hashpassword)
            return "success";
        return "fail";
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string inputPassword, string storedHash) //admin@123
    {
        return HashPassword(inputPassword) == storedHash;
    }

}
