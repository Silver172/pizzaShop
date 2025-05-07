using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess.ViewModels;

public class AddEditSectionViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9]+)*$", ErrorMessage = "Invalid name.")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public bool? Isdeleted { get; set; }

    public string? Description { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }
}
