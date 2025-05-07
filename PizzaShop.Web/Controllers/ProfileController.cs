using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.Data;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IJWTService _jwtService;
    private readonly IAuthenticationServices _authService;
    private readonly IProfileService _profileService;
    private readonly IAddressService _addressService;
    private readonly IUsersService _userService;
    private readonly IRolePermissionService _rolePermissionService;

    public ProfileController(IJWTService jwtService, IAuthenticationServices authService, IProfileService profileService, IAddressService addressService, IUsersService userService,IRolePermissionService rolePermissionService)
    {
        _jwtService = jwtService;
        _authService = authService;
        _profileService = profileService;
        _addressService = addressService;
        _userService = userService;
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");

        var (userEmail, role, userId) = _jwtService.ValidateToken(AuthToken);


        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));

            var userProfile = await _profileService.GetUserProfile(userEmail);
            
            await populateDropDown(userProfile.Country,userProfile.State);
            return View(userProfile);
        }
        catch (System.Exception)
        {
            TempData["ToastrMessage"] = "There was an error loading your profile. Please try again.";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (userEmail, role, userId) = _jwtService.ValidateToken(AuthToken);

        if (ModelState.IsValid)
        {
            try
            {
                var userProfile = await _profileService.GetUserProfile(userEmail);

                bool isProfileUpdated = await _profileService.UpdateProfileByEmail(model, userId);

                await populateDropDown(model.Country,model.State);

                if (!isProfileUpdated)
                {
                    TempData["ToastrMessage"] = "Profile is not updated please try again";
                    TempData["ToastrType"] = "error";
                    return View(model);
                }
                TempData["ToastrMessage"] = "Profile updated successfully";
                TempData["ToastrType"] = "success";

                return View(model);
            }
            catch (System.Exception)
            {
                var userProfile = await _profileService.GetUserProfile(userEmail);
                await populateDropDown(userProfile.Country,userProfile.State);
                TempData["ToastrMessage"] = "Profile is not updated please try again";
                TempData["ToastrType"] = "error";
                return View();
            }

        }
        else
        {
            var userProfile = await _profileService.GetUserProfile(userEmail);
            await populateDropDown(userProfile.Country,userProfile.State);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetStates(int countryId)
    {
        var states = await _addressService.GetAllStates(countryId);
        return Json(states);
    }

    [HttpGet]
    public async Task<JsonResult> GetCities(int stateId)
    {
        var cities = await _addressService.GetAllCities(stateId);
        return Json(cities);
    }

    public async Task<ActionResult> ChangePassword()
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");
        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
        }
        catch (System.Exception)
        {
            TempData["ToastrMessage"] = "Error while";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Dashboard", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {

            var AuthToken = Request.Cookies["AuthToken"];
            var (email, role, userId) = _jwtService.ValidateToken(AuthToken);

            var account = await _authService.FindAccount(email);
            if (account == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            try
            {
                string changePassword = await _profileService.ChangePassword(model, email);
                if (changePassword == "success")
                {
                    TempData["ToastrMessage"] = "Password changed successfully";
                    TempData["ToastrType"] = "success";
                    // return RedirectToAction("Dashboard", "Dashboard");
                }
                if (changePassword == "user not found")
                { ViewData["WrongPassword"] = "Something went wrong please try again !"; }
                else if (changePassword == "New password must be different from current password")
                {
                    TempData["ToastrMessage"] = "New password must be different from current password";
                    TempData["ToastrType"] = "warning";
                    return View();
                }
                else if (changePassword == "Current Password Is Incorrect")
                { ViewData["WrongPassword"] = "Incorrect current password"; }
                else if (changePassword == "fail")
                { ViewData["WrongPassword"] = "Something went wrong please try again"; }

            }
            catch (System.Exception)
            {
                TempData["ToastrMessage"] = "Error while change the password please try again";
                TempData["ToastrType"] = "error";
            }
        }
        return View();
    }

    [HttpGet]
    public async Task<JsonResult?> GetUserProfile()
    {
        string authToken = Request.Cookies["AuthToken"]; ;
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        if (!string.IsNullOrEmpty(email))
        {
            var user = await _userService.GetUserByEmail(email);
            return Json(new { profilePhoto = user.Profileimage, userName = user.Username });
        }
        return null;
    }

    private async Task populateDropDown(string? countryId,string? stateId)
    {
        ViewBag.AllCountries = await _addressService.GetAllCountries();
        ViewBag.AllCities = await _addressService.GetAllCities(string.IsNullOrEmpty(stateId) ? 0 : int.Parse(stateId));
        ViewBag.AllStates = await _addressService.GetAllStates(string.IsNullOrEmpty(countryId) ? 0 : int.Parse(countryId));
    }
}
