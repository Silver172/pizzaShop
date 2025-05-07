using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITaxesAndFeesService
{
    public Task<List<TaxesAndFeesViewModel>> GetTexesAndFees(PaginationViewModel pagination);
    public Task<int> GetTaxesAndFeesCount(string searchString);
    public Task<AddEditTaxesAndFeesViewModel> GetTaxDetails(int? id);
    public Task<bool> AddNewTax(AddEditTaxesAndFeesViewModel model, string userId);
    public Task<bool> EditTax(AddEditTaxesAndFeesViewModel model,string userId);
    public Task DeleteTax(int id,string userId);
}
