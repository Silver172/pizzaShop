using PizzaShop.DataAccess.Data;

namespace PizzaShop.Service.Interfaces;

public interface IAuthenticationServices
{
    public Task<Account?> VerifyUser(string email, string password);
    public void SendMail(string ToEmail, string subject, string body);
    public string? ValidateResetToken(string token);
    public string GenerateResetToken(string email);
    public Task<bool> UpdatePassword(string email, string password);
    public Task<Account> FindAccount(string email);
    public Task MarkUserFirstLogin(string email);
}
