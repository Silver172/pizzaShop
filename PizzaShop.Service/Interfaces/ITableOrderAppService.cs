using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableOrderAppService
{
    public Task<TableViewOrderAppViewModel> GetTableView();
    public Task<AddEditWaitingTokenViewModel> AssignTableOffcanvasData(int? id);
    public Task<List<Table>> GetOptimizedTables(AddEditWaitingTokenViewModel model,List<int> tableIds);
    public Task<int> AssignTable(AddEditWaitingTokenViewModel model, string userId, List<int> tableIds);
}
