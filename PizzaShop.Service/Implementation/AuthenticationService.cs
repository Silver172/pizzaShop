using System.Net.Http.Headers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace PizzaShop.Service.Implementation;

public class AuthenticationService : IAuthenticationServices
{
    private readonly IDataProtector _dataProtector;
    private readonly IAccountRepository _account;

    public AuthenticationService(IAccountRepository account, IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("ResetPasswordProtector");
        _account = account;
    }

    public async Task<Account?> VerifyUser(string email, string password)
    {
        var account = await _account.GetAccountByEmail(email);
        if(account == null)
            return null;
        if (VerifyPassword(password, account.Password))
        {
            return account;
        }
        return null;
    }

    public async Task MarkUserFirstLogin(string email)
    {
        await _account.MarkUserFirstLogin(email);
    }

    public async Task<bool> UpdatePassword(string email, string password)
    {
        string hashPassword = HashPassword(password);
        var account = await _account.UpdatePassword(email, hashPassword);
        if (account.Password == hashPassword)
            return true;
        return false;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        // return HashPassword(inputPassword) == storedHash;
        return true;
    }

    public string GenerateResetToken(string email)
    {
        DateTime expiry = DateTime.UtcNow.AddHours(24);
        string tokenData = $"{email} | {expiry.Ticks}";
        return _dataProtector.Protect(tokenData);       //encrypted token
    }

    public string ValidateResetToken(string token)
    {
        string unprotectedToken;
        try
        {
            //decrypt the token
            unprotectedToken = _dataProtector.Unprotect(token);
        }
        catch
        { return null; }

        //token has {email} | {expiryticks}
        var parts = unprotectedToken.Split('|');
        if (parts.Length != 2 || !long.TryParse(parts[1], out long expiryTicks))
            return null;

        DateTime expiryDate = new DateTime(expiryTicks, DateTimeKind.Utc);      //converts expiry ticks into datetime object
        if (expiryDate < DateTime.UtcNow)
            return null;

        string email = parts[0].Trim();
        return email;
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
        // StringBuilder mailBody = new StringBuilder();
        mailMessage.Body = body;

        smtpClient.Send(mailMessage);
    }

    public async Task<Account> FindAccount(string email)
    {
        return await _account.GetAccountByEmail(email);
    }

}