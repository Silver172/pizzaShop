using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableAndSectionService
{
    public Task<TableAndSectionViewModel> GetTableSectionPage();
    public Task<List<SectionViewModel>> GetAllSection();
    public Task<List<TableViewModel>> GetTablesBySectionId(int id, PaginationViewModel pagination);
    public Task<TableAndSectionViewModel?> AddSection(AddEditSectionViewModel model,string userId);
    public Task<AddEditSectionViewModel> GetSectionById(int id);
    public Task<TableAndSectionViewModel?> EditSection(AddEditSectionViewModel model,string userId);
    public Task<TableAndSectionViewModel?> DeleteSection(int id,string userId);
    public Task<AddEditTableViewModel> GetTableById(int? id);
    public Task<int> GetTablesCountbySectionId(int id,string? searchString);
    public Task<bool?> AddTable(AddEditTableViewModel model,string userId);
    public Task<bool?> EditTable(AddEditTableViewModel model,string userId);
    public Task<bool> DeleteTable(int id,string userId);
    public Task<bool> DeleteMultipleTable(int[] ids,string userId);
}
