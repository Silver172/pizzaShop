namespace PizzaShop.DataAccess.ViewModels;

public class ModifierTabViewModel
{
    public List<ModifierGroupViewModel>? ModifierGroupList { get; set; }
    public List<ModifierViewModel>? ModifierList { get; set; }
    public List<ModifierViewModel>? AllModifiers {get; set;}
    public AddEditModifierViewModel? AddEditModifier {get; set;}
    public PaginationViewModel? Pagination {get; set;}
}
