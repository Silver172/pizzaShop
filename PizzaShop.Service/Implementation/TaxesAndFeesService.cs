using System.Threading.Tasks;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class TaxesAndFeesService : ITaxesAndFeesService
{
    private readonly ITaxesAndFeesRepository _tnfRepository;

    public TaxesAndFeesService(ITaxesAndFeesRepository tnfRepository)
    {
        _tnfRepository = tnfRepository;
    }

    public async Task<List<TaxesAndFeesViewModel>> GetTexesAndFees(PaginationViewModel pagination)
    {
        return await _tnfRepository.GetTaxesAndFees(pagination);
    }

    public async Task<int> GetTaxesAndFeesCount(string searchString)
    {
        return await _tnfRepository.GetTaxesAndFeesCount(searchString);
    }


    public async Task<AddEditTaxesAndFeesViewModel> GetTaxDetails(int? id)
    {
        var tax = await _tnfRepository.GetTaxesandfeesById(id);
        var editTaxDetail = new AddEditTaxesAndFeesViewModel
        {
            Id = tax.Id,
            Name = tax.Name,
            TaxValue = (decimal)(tax.Flatamount == null ? tax.Percentge : tax.Flatamount),
            Type = tax.Flatamount == null ? "Percentage" : "Flat Amount",
            Isactive = (bool)tax.Isactive,
            Isdefault = (bool)tax.Isdefault
        };
        return editTaxDetail;
    }

    public async Task<bool> AddNewTax(AddEditTaxesAndFeesViewModel model, string userId)
    {
        bool isExist = await _tnfRepository.IsTaxExist(model.Name);
        if (isExist)
            return false;
        await _tnfRepository.AddNewTax(model, userId);
        return true;
    }

    public async Task<bool> EditTax(AddEditTaxesAndFeesViewModel model,string userId)
    {
        bool isExist = await _tnfRepository.IsEditTaxExist(model.Name,model.Id);
        if(isExist)
            return false;
        await _tnfRepository.EditTax(model,userId);
        return true;
    }

    public async Task DeleteTax(int id,string userId)
    {
        await _tnfRepository.DeleteTax(id,userId);
    }
}
