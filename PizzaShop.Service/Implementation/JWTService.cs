namespace PizzaShop.Service.Implementation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;

public class JWTService : IJWTService
{
    private readonly IConfiguration _config;
    private readonly IRoleRepository _role;
    private readonly IUsersService _usersService;
    private readonly IUserRepository _userRepository;
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JWTService(IConfiguration config, IRoleRepository role, IUsersService usersService,IUserRepository userRepository)
    {
        _config = config;
        _role = role;
        _key = config["Jwt:Key"]!;
        _issuer = config["Jwt:Issuer"]!;
        _audience = config["Jwt:Audience"]!;
        _usersService = usersService;
        _userRepository = userRepository;
    }


    public async Task<string> GenerateToken(string email, int? role, bool rememberMe)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var userRole = await _role.GetRoleById(role);
        var user = await _usersService.GetUserByEmail(email);
        var authClaims = new List<Claim>{
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, userRole),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
         };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: authClaims,
            expires: rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string?, string?, string?) ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value.Trim();
            var role = principal.FindFirst(ClaimTypes.Role)?.Value.Trim();
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Trim().ToString();
            return (email, role, userId);
        }
        catch (Exception ex)
        {
            return (null, null, null);
        }
    }
}
