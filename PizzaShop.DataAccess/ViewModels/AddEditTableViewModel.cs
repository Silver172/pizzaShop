using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess.ViewModels;

public class AddEditTableViewModel
{
  public int Id { get; set; }
  [Required(ErrorMessage = "Section is required")]
  public int? Sectionid { get; set; }
  [Required(ErrorMessage = "Name is Required")]
  [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9]+)*$", ErrorMessage = "Invalid name.")]
  [MaxLength(10)]
  public string Name { get; set; } = null!;
  [Required(ErrorMessage = "Capacity is required")]
  [Range(1, 50 , ErrorMessage = "Capacity must be between 1 and 50")]
  [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid capacity.")]
  public short Capacity { get; set; }

  public string Status { get; set; } = null!;

  public bool? Isavailable { get; set; }

  public DateTime? Createddate { get; set; }

  public string? Createdby { get; set; }

  public DateTime? Updateddate { get; set; }

  public string? Updatedby { get; set; }

  public List<SectionViewModel>? Sections {get; set;} = null!;
}