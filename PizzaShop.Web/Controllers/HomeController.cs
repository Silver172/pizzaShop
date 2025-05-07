using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Models;

namespace PizzaShop.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJWTService _jwtService;

    public HomeController(ILogger<HomeController> logger, IJWTService jwtService)
    {
        _logger = logger;
        _jwtService = jwtService;
    }

    public IActionResult Index()
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Error404()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Error401()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Error403()
    {
        return View();
    }
}
