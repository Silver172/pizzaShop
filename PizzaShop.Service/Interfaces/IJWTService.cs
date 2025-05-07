using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Service.Interfaces;


public interface IJWTService
{
    public Task<string> GenerateToken(string email, int? role, bool rememberMe);
    public (string?, string?, string?) ValidateToken(string token);
}
