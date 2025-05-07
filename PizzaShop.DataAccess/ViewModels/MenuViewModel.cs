using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess.ViewModels;

public class MenuViewModel
{
    public ItemTabViewModel ItemsTab { get; set; }

    public ModifierTabViewModel ModifierTab { get; set; }
}
