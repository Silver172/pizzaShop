using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.DataAccess.Data;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.Web.Controllers;

public class AuthenticationController : Controller
{
    private readonly IAuthenticationServices _authService;
    private readonly IJWTService _IJWTService;
    private readonly IUsersService _userService;
    public AuthenticationController(IDataProtectionProvider dataProtectionProvider, IJWTService IJWTService, IAuthenticationServices iAuthService, IUsersService userService)
    {
        _IJWTService = IJWTService;
        _authService = iAuthService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var AuthToken = Request.Cookies["AuthToken"];

        if (!string.IsNullOrEmpty(AuthToken))
        {
            var (email, role, userId) = _IJWTService.ValidateToken(AuthToken);
            if (email != null && role != null)
                return RedirectToAction("Dashboard", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (ModelState.IsValid)
        {
            var account = await _authService.VerifyUser(login.Email, login.Password);

            if (account != null)
            {
                if (account.Isfirstlogin == null || account.Isfirstlogin == false)
                {
                    await _authService.MarkUserFirstLogin(account.Email);
                }
                var user = await _userService.GetUserByEmail(account.Email);

                if (user.Isactive != true)
                {
                    TempData["ToastrMessage"] = "Your account is inactive. Access is restricted.";
                    TempData["ToastrType"] = "warning";
                    return View();
                }
                // create JWT Token 
                string token = await _IJWTService.GenerateToken(account.Email, account.Roleid, login.RememberMe);

                if (string.IsNullOrEmpty(token))
                {
                    return View();
                }
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(3),
                });

                var option = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(3),
                    Secure = true
                };
                Response.Cookies.Append("Email", account.Email, option);

                // Store Jwt Authentication Token
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                if (login.RememberMe)
                {
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(30),
                    });
                }
                TempData["ToastrMessage"] = "Login successfully";
                TempData["ToastrType"] = "success";
                if(user.Roleid == 3)
                    return RedirectToAction("Index", "KOTOrderApp");
                else
                    return RedirectToAction("Dashboard", "Dashboard");
            }
            else
            {
                TempData["ToastrMessage"] = "Wrong credentials please try again";
                TempData["ToastrType"] = "error";
                return View(login);
            }
        }
        return View(login);
    }

    [HttpGet]
    public ActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var account = await _authService.FindAccount(model.Email);
                if (account == null)
                {
                    TempData["ToastrMessage"] = "Account not found please check your mail";
                    TempData["ToastrType"] = "error";
                    return View(model);
                }

                string resetToken = _authService.GenerateResetToken(model.Email);
                var resetLink = this.Url.Action("ResetPassword", "Authentication", new { token = resetToken }, Request.Scheme);
                Console.WriteLine(resetLink);
                Response.Cookies.Append("ResetToken", resetToken, new CookieOptions
                {
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true
                });
                string subject = "Password reset request";
                string body = GetEmailTemplate(resetLink);

                _authService.SendMail(model.Email, subject, body);
                TempData["ToastrMessage"] = "Mail sent successfully !";
                TempData["ToastrType"] = "success";
                return RedirectToAction("ForgotPassword");
            }
            catch (System.Exception)
            {
                TempData["ToastrMessage"] = "Something went wrong please try again";
                TempData["ToastrType"] = "error";
            }

        }
        return View(model);
    }

    private static string GetEmailTemplate(string ResetLink)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "EmailTemplate.html");
        if (!System.IO.File.Exists(templatePath))
        {
            return "<p>Email template Not Fount</p>";
        }
        string emailbody = System.IO.File.ReadAllText(templatePath);
        return emailbody.Replace("{{Link}}", ResetLink);
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var ResetToken = Request.Cookies["ResetToken"];
        if (string.IsNullOrEmpty(ResetToken))
            return RedirectToAction("ResetDeny", "Authentication");
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid reset token");
        }
        var model = new ResetPasswordViewModel { Token = token };
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetDeny()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                string email = _authService.ValidateResetToken(model.Token);

                if (string.IsNullOrEmpty(email))
                {
                    TempData["ToastrMessage"] = "Token is expired or invalid token please try again";
                    TempData["ToastrType"] = "error";
                    return RedirectToAction("Login", "Authentication");
                }

                bool update = await _authService.UpdatePassword(email, model.NewPassword);

                if (update)
                {
                    TempData["ToastrMessage"] = "Password updated successfully";
                    TempData["ToastrType"] = "success";
                    Response.Cookies.Delete("ResetToken");
                    return RedirectToAction("Login", "Authentication");
                }

                return View();
            }
            catch (System.Exception)
            {
                TempData["ToastrMessage"] = "Something went wring please try again";
                TempData["ToastrType"] = "success";
                return View();
            }
        }
        else
        {
            return View();
        }
    }

    public IActionResult Logout()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("Email");
        return RedirectToAction("Login", "Authentication");
    }
}
