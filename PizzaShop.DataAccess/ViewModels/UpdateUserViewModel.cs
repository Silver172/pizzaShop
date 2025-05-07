using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.DataAccess.Data;
namespace PizzaShop.ViewModels;

public class UpdateUserViewModel
{
    [Required(ErrorMessage ="User Id is required")]
    public int Id { get; set; }

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
    public string Phone { get; set; } = null!;

    public string Email { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = null!;

    public string? Profileimage { get; set; }

    public IFormFile? ProfileimagePath { get; set; }

    public string? Country { get; set; } = null!;

    public string? State { get; set; } = null!;

    public string? City { get; set; } = null!;

    [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Zipcode must be 6 digits and valid.")]
    public string? Zipcode { get; set; }

    public string? Address { get; set; } = null!;

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }

    public List<Role>? Roles {get;set;}
}
