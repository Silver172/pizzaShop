namespace PizzaShop.DataAccess.ViewModels;

public class SectionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Isdeleted { get; set; }

    public string? Description { get; set; }

    public int? waitingTokenCount { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }
}
