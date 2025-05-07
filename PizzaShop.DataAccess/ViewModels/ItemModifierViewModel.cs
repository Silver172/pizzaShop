namespace PizzaShop.DataAccess.ViewModels;

public class ItemModifierViewModel
{
  public int? Id {get; set;}
  public string? Name {get; set;}
  public int ModifierGroupId {get;set;}
  public int? Minselectionrequired {get;set;}
  public int? Maxselectionrequired {get; set;}

  public List<ModifierViewModel>? ModifierList {get;set;}
}
