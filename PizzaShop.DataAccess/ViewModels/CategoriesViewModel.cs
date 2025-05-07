using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess.ViewModels;

public class CategoriesViewModel
{
    
    public int Id { get; set; }

    [Required(ErrorMessage = "Category Name is required Field")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Description is required Field")]
    public string Description { get; set; } = null!;

    public bool? Isdeleted { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }
}
