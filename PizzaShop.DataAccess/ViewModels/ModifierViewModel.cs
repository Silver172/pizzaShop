namespace PizzaShop.DataAccess.ViewModels;

public class ModifierViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public short Quantity { get; set; }

    public bool? Isdeleted { get; set; }

    public string Description { get; set; } = null!;

    public int? Modifiergroupid { get; set; }

    public int? Unitid { get; set; }

    public string? Unit {get; set;} 
     
}
