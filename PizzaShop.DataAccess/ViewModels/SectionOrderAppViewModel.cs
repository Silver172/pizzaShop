namespace PizzaShop.DataAccess.ViewModels;

public class SectionOrderAppViewModel
{
     public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Available {get;set;}
    public int Assigned {get;set;}
    public int Running {get;set;}
    public List<TableOrderAppViewModel>? TableList {get;set;}
}
