using System.ComponentModel.DataAnnotations;
using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.ViewModels;

public class AddEditModifierViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9]+)*$", ErrorMessage = "Invalid name.")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Rate.")]
    [Range(0.01, 99999999.99, ErrorMessage = "Invalid Rate.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [RegularExpression("([0-9]+)",ErrorMessage = "Invalid Quantity")] 
    public short Quantity { get; set; }

    public bool? Isdeleted { get; set; }

    public string? Description { get; set; } = null!;

    [Required(ErrorMessage = "ModifierGroup is required.")]
    public int Modifiergroupid { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int? Unitid { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }

    public List<ModifierGroupViewModel>? ModifierGroups { get; set; }

    public List<Unit>? Units { get; set; }
}
