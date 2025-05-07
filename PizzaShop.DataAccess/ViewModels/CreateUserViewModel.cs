using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.DataAccess.Data;
namespace PizzaShop.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "First Name is required.")]
    [MaxLength(50,ErrorMessage ="Fist Name should be less then 50 character")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "First Name can only contain letters.")]
    public string Firstname { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required")]
    [MaxLength(50,ErrorMessage ="Last name should be less then 50 character")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Last Name can only contain letters.")]
    public string? Lastname { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    [MaxLength(50,ErrorMessage ="Username should be less then 50 character")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "User name can only contain letters, numbers, and underscores.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Please enter a valid phone number.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(50,ErrorMessage ="Email should be less then 50 character")]
    [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "Invalid Email")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required"), DataType(DataType.Password), MinLength(8)]
    [MaxLength(50,ErrorMessage ="Password should be less then 200 character")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include uppercase, lowercase, number and special character.")]
    public string Password { get; set; } = null!;

    public string? Profileimage { get; set; }
    public IFormFile? ProfileimagePath { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Zipcode must be 6 digits and valid.")]
    public string? Zipcode { get; set; }

    public string? Address { get; set; }

    public string Role { get; set; }

    public int? Createdby { get; set; }

    public List<Role>? Roles {get;set;}
}