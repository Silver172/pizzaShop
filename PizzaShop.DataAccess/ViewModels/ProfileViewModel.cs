using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "FirstName is required.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName can only contain letters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "LastName can only contain letters.")]
    public string LastName { get; set; } = null!;

    public string? Email {get;set;}

    [Required(ErrorMessage = "UserName is required")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "User name can only contain letters, numbers, and underscores.")]
    public string UserName { get; set; } = null!;
    
    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Please enter a valid phone number.")]
    public string Phone { get; set; } = null!;

    // [Required(ErrorMessage = "Country is required")]
    public string? Country { get; set; } = null!;

    // [Required(ErrorMessage = "State is required")]
    public string? State { get; set; } = null!;

    // [Required(ErrorMessage = "City is required")]
    public string? City { get; set; } = null!;

    // [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; } = null!;

    // [Required(ErrorMessage = "ZipCode is required")]
    [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Zipcode must be 6 digits and valid.")]
    public string? ZipCode { get; set; } = null!;

    public IFormFile? ProfileImage {get; set;} = null!;
    public string? ProfileImagePath {get; set;} = null!;
    public string? Role { get; set; }
}