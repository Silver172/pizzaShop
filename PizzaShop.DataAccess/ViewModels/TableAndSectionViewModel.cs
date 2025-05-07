namespace PizzaShop.DataAccess.ViewModels;

public class TableAndSectionViewModel
{
    public List<TableViewModel> TableList { get; set; } = null!;
    public List<SectionViewModel> SectionList { get; set; } = null!;
    public AddEditSectionViewModel? AddEditSection {get; set;}
    public PaginationViewModel Pagination = null!;
}
