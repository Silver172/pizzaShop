using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ITaxesAndFeesRepository
{
    public Task<List<TaxesAndFeesViewModel>> GetTaxesAndFees(PaginationViewModel pagination);
    public Task<int> GetTaxesAndFeesCount(string searchString);
    public Task<Taxesandfee?> GetTaxesandfeesById(int? id);
    public Task AddNewTax(AddEditTaxesAndFeesViewModel model,string userId);
    public Task EditTax(AddEditTaxesAndFeesViewModel model,string userId);
    public Task<bool> IsTaxExist(string name);
    public Task<bool> IsEditTaxExist(string name, int? id);
    public Task DeleteTax(int id,string userId);
    public Task<List<Taxesandfee>> GetAllTaxesAndFees();
}
