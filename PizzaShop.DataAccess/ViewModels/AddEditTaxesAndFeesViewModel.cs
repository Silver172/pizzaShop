using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess.ViewModels;

public class AddEditTaxesAndFeesViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid name.")]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage ="Type is required")]
    public string Type {get;set;}

    [Required(ErrorMessage = "Tax value is required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid tax value")]
    public decimal TaxValue { get; set; }

    public bool Isactive { get; set; }

    public bool Isdefault { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }
}
