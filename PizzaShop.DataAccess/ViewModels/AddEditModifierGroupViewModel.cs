namespace PizzaShop.DataAccess.ViewModels;

public class AddEditModifierGroupViewModel
{
    public int? Id {get; set;} 
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<ExistingModifierViewModel>? Modifiers { get; set; }
}