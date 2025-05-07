using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.DataAccess.Data;
namespace PizzaShop.DataAccess.ViewModels;

public class AddEditItemViewModel
{
    public int id { get; set; }
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9]+)*$", ErrorMessage = "Invalid name.")]
    [MaxLength(20)]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "required")]
    public string Itemtype { get; set; } = null!;
    
    [Required(ErrorMessage = "Rate is required")]
    [Range(1, 9999999.99, ErrorMessage = "Invalid rate")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid rate")]
    public decimal Rate { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public short Quantity { get; set; }
    public bool Isavailable { get; set; }
    
    [DefaultValue(false)]
    public bool Isdeleted { get; set; }
    
    [Required(ErrorMessage = "Unit is required")]
    public int Unitid { get; set; }
    
    [Required(ErrorMessage = "Is Default Tax is required")]
    public bool Isdefaulttax { get; set; }

    [Range(0, 100.00, ErrorMessage = "Invalid Tax percentage")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid tax percentage")]
    public decimal? Taxpercentage { get; set; }
    
    [MaxLength(10)]
    public string? Shortcode { get; set; }
    
    public string? Description { get; set; }

    public IFormFile? Itemimage { get; set; }
    public string? ImagePath { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }
    public List<ItemModifierViewModel>? ItemModifiers { get; set; }
    public int []? ModifierGroupIds { get; set; }
    public List<ModifierGroupViewModel>? ModifierGroups { get; set; }
    public List<Unit>? Units { get; set; }
    public List<CategoriesViewModel>? Categories { get; set; }
}

