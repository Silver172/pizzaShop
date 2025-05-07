using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ITableRepository
{
    public Task<List<TableViewModel>> GetAllTables();
    public Task<List<TableViewModel>> GetTablesBySectionId(int id, PaginationViewModel pagination);
    public Task<int> GetTablesCount(int id,string? searchString);
    public Task<bool> IsTableExist(string name, int? sectionId);
    public Task<bool> IsEditTableExist(string name, int? id, int? sectionId);
    public Task<bool> DeleteTableBySectionId(int id,string userId);
    public Task AddTable(AddEditTableViewModel model, string userId);
    public Task EditTable(AddEditTableViewModel model, string userId);
    public Task<bool> DeleteTable(int id,string userId);
    public Task<Table?> GetTableById(int? id);
    public Task<bool> DeleteMultipleTable(int[] ids,string userId);
    public Task<List<Table>> GetTablesBySectionIdForWaitingtoken(int id);
    public Task<List<Table>> GetOptimizedTablesBySectionId(int? id,int noOfPersons,List<int> tableIds);
    public Task UpdateTableStatus(int id, string status, string userId);
    public Task<List<Table>> GetAvailableTablesBySectionId(int id);
}
