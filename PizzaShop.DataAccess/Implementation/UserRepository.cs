using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class UserRepository : IUserRepository
{
    private readonly PizzashopContext _context;
    private readonly IRoleRepository _role;

    public UserRepository(PizzashopContext context, IRoleRepository role)
    {
        _context = context;
        _role = role;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> UpdateUserProfileByEmail(ProfileViewModel model, string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        user.Firstname = model.FirstName.Trim();
        user.Lastname = model.LastName.Trim();
        user.Username = model.UserName.Trim();
        user.Phone = model.Phone;
        user.City = model.City != null ? model.City : "";
        user.State = model.State != null ? model.State : "";
        user.Country = model.Country != null ? model.Country : "";
        user.Address = model.Address != null ? model.Address.Trim() : "";
        user.Zipcode = model.ZipCode;
        user.Profileimage = model.ProfileImagePath;
        user.Updatedby = userId;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<UsersViewModel>> GetUsers(string searchString, int pageIndex, int pageSize, string? sortBy, string? sortType)
    {
        var userQuery = _context.Users.Where(u => u.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();

            userQuery = userQuery.Where(n =>
                (n.Firstname + " " + n.Lastname).ToLower().Contains(searchString) || // Full name search
                n.Firstname.ToLower().Contains(searchString) || 
                n.Lastname.ToLower().Contains(searchString) || 
                n.Phone.Contains(searchString));
        }

        var userList = new List<UsersViewModel>();
         var usersList = new List<User>();
        if (sortType == "ascending")
        {
            usersList = await userQuery
            .OrderBy<User, object>(o => sortBy == "Role" ? o.Role.Name :  o.Firstname).ThenBy(u => u.Id)
            // .OrderBy(u => u.Firstname).ThenBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }else{
            usersList = await userQuery
            .OrderByDescending<User, object>(o => sortBy == "Role" ? o.Role.Name :  o.Firstname).ThenBy(u => u.Id)
            // .OrderBy(u => u.Firstname).ThenBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }
        

        foreach (var user in usersList)
        {
            userList.Add(new UsersViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Profileimage = user.Profileimage,
                Email = user.Email,
                Phone = user.Phone,
                Country = user.Country,
                State = user.State,
                City = user.City,
                Zipcode = user.Zipcode,
                Address = user.Address,
                Role = await _role.GetRoleById(user.Roleid),
                Isactive = user.Isactive == true ? "Active" : "InActive",
                Username = user.Username
            });
        }
        return userList;
    }

    public async Task<int> getUsersCount(string searchString)
    {
        var userQuery = _context.Users.Where(u => u.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();

            userQuery = userQuery.Where(n =>
                (n.Firstname + " " + n.Lastname).ToLower().Contains(searchString) || // Full name search
                n.Firstname.ToLower().Contains(searchString) || 
                n.Lastname.ToLower().Contains(searchString) || 
                n.Phone.Contains(searchString));
        }
        var users = await userQuery.ToListAsync();
        return users.Count();
    }

    public async Task CreateUser(CreateUserViewModel model, string userId)
    {
        var newUser = new User
        {
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Username = model.Username,
            Roleid = _context.Roles.FirstOrDefault(r => r.Id == int.Parse(model.Role))?.Id,
            Email = model.Email,
            Country = model.Country != null ? model.Country : "",
            State = model.State != null ? model.State : "",
            City = model.City != null ? model.City : "",
            Zipcode = model.Zipcode,
            Address = model.Address != null ? model.Address : "",
            Phone = model.Phone != null ? model.Phone : "",
            Profileimage = model.Profileimage,
            Createdby = userId,
            Createddate = DateTime.Now
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }

    public async Task<UpdateUserViewModel> GetUpdateUserDetail(int id)
    {
        var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

        var updateUser = new UpdateUserViewModel
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Email = user.Email,
            Address = user.Address,
            City = user.City,
            State = user.State,
            Country = user.Country,
            Zipcode = user.Zipcode,
            Phone = user.Phone,
            Profileimage = user.Profileimage,
            Role = user.Roleid.ToString(),
            Updatedby = user.Id.ToString(),
            Updateddate = DateTime.Now,
            Status = user.Isactive == true ? "1" : "0"
        };

        return updateUser;
    }

    public async Task UpdateUser(UpdateUserViewModel model, string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
        
        if (user != null)
        {
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Username = model.Username;
            user.Address = model.Address != null ? model.Address : "";
            user.Phone = model.Phone;
            user.City = model.City != null ? model.City : "";
            user.State = model.State != null ? model.State : "";
            user.Country = model.Country != null ? model.Country : "";
            user.Zipcode = model.Zipcode;
            user.Isactive = model.Status == "1" ? true : false;
            user.Roleid = int.Parse(model.Role);
            user.Updateddate = DateTime.Now;
            user.Updatedby = userId;
            user.Profileimage = model.Profileimage;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUser(int id, string userId)   //here we take role or userId
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        user.Isdeleted = true;
        user.Updatedby = userId;
        user.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

}
