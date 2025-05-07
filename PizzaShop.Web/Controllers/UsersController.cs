using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[PermissionAuthorize("Users.View")]
[Authorize]
public class UsersController : Controller
{
    private readonly IUsersService _user;
    private readonly IAccountRepository _account;
    private readonly IAddressService _addressService;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public UsersController(IUsersService user, IAccountRepository account, IAddressService addressService, IJWTService jwtService, IRolePermissionService rolePermissionService)
    {
        _user = user;
        _account = account;
        _addressService = addressService;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("Users.View")]
    [HttpGet]
    public async Task<IActionResult> Index(string searchString, int pageIndex = 1, int pageSize = 5, string? sortBy = "Name", string? sortType = "ascending")
    {

        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));

        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }

        var count = await _user.GetUsersCount(searchString);
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(count / (double)pageSize),
            TotalRecord = count,
            SortingBy = sortBy,
            SortingType = sortType
        };
        var userList = await _user.GetUsers(searchString, pageIndex, pageSize, sortBy, sortType);
        var userListPage = new UserPageViewModel
        {
            UserList = userList,
            Pagination = pagination
        };

        return View(userListPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(string searchString, int pageIndex, int pageSize, string? sortBy = "Name", string? sortType = "ascending")
    {
        var count = await _user.GetUsersCount(searchString);
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(count / (double)pageSize),
            TotalRecord = count,
            SortingBy = sortBy,
            SortingType = sortType
        };
        var userList = await _user.GetUsers(searchString, pageIndex, pageSize, sortBy, sortType);
        var userListPage = new UserPageViewModel
        {
            UserList = userList,
            Pagination = pagination
        };
        return PartialView("_UserList", userListPage);
    }

    [PermissionAuthorize("Users.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> CreateUser()
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (String.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));

            var AllCountries = await _addressService.GetAllCountries();
            var AllStates = await _addressService.GetAllStates(0);
            var AllCities = await _addressService.GetAllCities(0);
            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;
            List<Role> roles = await _user.GetAllRoles();
            CreateUserViewModel model = new CreateUserViewModel();
            model.Roles = roles;
            return View(model);
        }
        catch (System.Exception)
        {
            TempData["ToastrMessage"] = "Error while fetching the user data";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index","Users");
        }

    }

    [HttpGet]
    public async Task<JsonResult> GetStates(int countryId)
    {
        var states = await _addressService.GetAllStates(countryId);
        ViewBag.AllStates = states;
        return Json(states);
    }

    [HttpGet]
    public async Task<JsonResult> GetCities(int stateId)
    {
        var cities = await _addressService.GetAllCities(stateId);
        ViewBag.AllCities = cities;
        return Json(cities);
    }

    [PermissionAuthorize("Users.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var authToken = Request.Cookies["AuthToken"];
            if (String.IsNullOrEmpty(authToken))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var (email, role, userId) = _jwtService.ValidateToken(authToken);

            try
            {
                var user = await _account.GetAccountByEmail(model.Email);
                if (user != null)
                {
                    await populateDropDown();
                    List<Role> roles = await _user.GetAllRoles();
                    model.Roles = roles;
                    TempData["ToastrMessage"] = "User already exist";
                    TempData["ToastrType"] = "error";

                    return View(model);
                }
                bool newUser = await _user.CreateUser(model, userId);
                if (newUser)
                {
                    string body = GetEmailTemplate(model.Email, model.Password);
                    SendMail(email, "New Account", body);
                    TempData["ToastrMessage"] = "User created successfully";
                    TempData["ToastrType"] = "success";
                    return RedirectToAction("Index", "Users");
                }
            }
            catch (System.Exception)
            {
                TempData["ToastrMessage"] = "Error while adding the user";
                TempData["ToastrType"] = "error";
                await populateDropDown();
                List<Role> roles = await _user.GetAllRoles();
                model.Roles = roles;
                return View(model);
            }

        }
        await populateDropDown();
        return View();
    }

    private static string GetEmailTemplate(string email, string password)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "UserEmailTemplate.html");
        if (!System.IO.File.Exists(templatePath))
        {
            return "<p>Email template Not Fount</p>";
        }
        string emailbody = System.IO.File.ReadAllText(templatePath);
        emailbody = emailbody.Replace("{{Email}}", email);
        emailbody = emailbody.Replace("{{Password}}", password);
        return emailbody;
    }

    [PermissionAuthorize("Users.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> UpdateUser(int Id)
    {
        var authToken = Request.Cookies["AuthToken"];
        if (String.IsNullOrEmpty(authToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var (email, role, userId) = _jwtService.ValidateToken(authToken);

        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
            
            UpdateUserViewModel updateUser = await _user.GetUpdateUserDetail(Id);
            ViewBag.AllCountries = await _addressService.GetAllCountries();
            ViewBag.AllCities = await _addressService.GetAllStates(string.IsNullOrEmpty(updateUser.Country) ? -1 : int.Parse(updateUser.Country));
            ViewBag.AllStates = await _addressService.GetAllCities(string.IsNullOrEmpty(updateUser.State) ? -1 : int.Parse(updateUser.State));
            return View(updateUser);

        }
        catch (System.Exception)
        {
            TempData["ToastrMessage"] = "Error while fetching the user data";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index","Users");
        }

    }

    [PermissionAuthorize("Users.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var authToken = Request.Cookies["AuthToken"];
            if (String.IsNullOrEmpty(authToken))
            {
                return RedirectToAction("Login", "Authentication");
            }
            var (email, role, userId) = _jwtService.ValidateToken(authToken);

            try
            {
                await _user.UpdateUser(model, userId);
                TempData["ToastrMessage"] = "User updated successfully";
                TempData["ToastrType"] = "success";
                return RedirectToAction("Index", "Users");
            }
            catch (System.Exception)
            {
                TempData["ToastrMessage"] = "Error while updating the user";
                TempData["ToastrType"] = "error";
                await populateDropDown();
                List<Role> roles = await _user.GetAllRoles();
                model.Roles = roles;
                return View(model);
            }
        }
        else
        {
            await populateDropDown();
            return View(model);
        }
    }

    [PermissionAuthorize("Users.Delete")]
    public async Task<IActionResult> DeleteUser(int Id)
    {
        var authToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(authToken);

        if (email == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        try
        {
            await _user.DeleteUser(Id, userId);
            TempData["ToastrMessage"] = "User deleted successfully";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index", "Users");
        }
        catch (System.Exception)
        {
            TempData["ToastrMessage"] = "User not deleted please try again";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index", "Users");
        }
    }


    public void SendMail(string ToEmail, string subject, string body)
    {
        string SenderMail = "test.dotnet@etatvasoft.com";
        string SenderPassword = "P}N^{z-]7Ilp";
        string Host = "mail.etatvasoft.com";
        int Port = 587;

        var smtpClient = new SmtpClient(Host)
        {
            Port = Port,
            Credentials = new NetworkCredential(SenderMail, SenderPassword),
        };

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(SenderMail);
        mailMessage.To.Add(ToEmail);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        // string mailBody = $"Hello Your Account has been created and your Temporary Password is {tempPassword}";
        mailMessage.Body = body;

        smtpClient.Send(mailMessage);
    }


    private async Task populateDropDown()
    {
        ViewBag.AllCountries = await _addressService.GetAllCountries();
        ViewBag.AllCities = await _addressService.GetAllStates(0);
        ViewBag.AllStates = await _addressService.GetAllCities(0);
    }

}
