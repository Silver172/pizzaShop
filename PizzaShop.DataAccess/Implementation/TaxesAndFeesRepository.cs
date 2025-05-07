using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class TaxesAndFeesRepository:ITaxesAndFeesRepository
{
    private readonly PizzashopContext _context;

    public TaxesAndFeesRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<TaxesAndFeesViewModel>> GetTaxesAndFees(PaginationViewModel pagination)
    {
         var query = _context.Taxesandfees.Where(m => m.Isdeleted == false);

        if (!string.IsNullOrEmpty(pagination.SearchString))
        {
            var searchString = pagination.SearchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchString));
        }

        var tnf = await query.OrderBy(i => i.Name)
            .Skip((pagination.PageIndex - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var tnfList = new List<TaxesAndFeesViewModel>();

        foreach (var i in tnf)
        {
            tnfList.Add(new TaxesAndFeesViewModel{
                Id = i.Id,
                Name = i.Name,
                Percentge = i.Percentge,
                Flatamount = i.Flatamount,
                Isactive = i.Isactive,
                Isdefault = i.Isdefault
            });
        }
        return tnfList;
    }

    public async Task<List<Taxesandfee>> GetAllTaxesAndFees()
    {
        var tnf = await _context.Taxesandfees.Where(m => m.Isdeleted == false && m.Isactive == true).ToListAsync();
        return tnf;
    }

    public async Task<int> GetTaxesAndFeesCount(string searchString)
    {
        var query = _context.Taxesandfees.Where(m => m.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            var SearchString = searchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(SearchString));
        }
        var tnf = await query.OrderBy(i => i.Name).ToListAsync();
        return tnf.Count();
    }

    public async Task<Taxesandfee?> GetTaxesandfeesById(int? id)
    {
        var tax = await _context.Taxesandfees.FirstOrDefaultAsync(i => i.Id == id);
        return tax;
    }

    public async Task AddNewTax(AddEditTaxesAndFeesViewModel model,string userId)
    {
        var newTax = new Taxesandfee{
            Name = model.Name,
            Percentge = model.Type == "Percentage" ? model.TaxValue : 0,
            Flatamount = model.Type == "Flat Amount" ? model.TaxValue : null,
            Isactive = model.Isactive,
            Isdefault = model.Isdefault,
            Isdeleted = false,
            Createdby = userId,
            Createddate = DateTime.Now
        };
        await _context.Taxesandfees.AddAsync(newTax);
        await _context.SaveChangesAsync();
    }

    public async Task EditTax(AddEditTaxesAndFeesViewModel model,string userId)
    {
        var tax = await _context.Taxesandfees.FirstOrDefaultAsync(t => t.Id == model.Id && t.Isdeleted == false);
        if(tax != null)
        {
            tax.Name = model.Name;
            tax.Percentge = model.Type == "Percentage" ? model.TaxValue : 0;
            tax.Flatamount = model.Type == "Flat Amount" ? model.TaxValue : null;
            tax.Isactive = model.Isactive;
            tax.Isdefault = model.Isdefault;
            tax.Updateddate = DateTime.Now;
            tax.Updatedby = userId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTax(int id,string userId)
    {
        var deleteTax = await _context.Taxesandfees.FirstOrDefaultAsync(i => i.Id == id);
        if(deleteTax != null)
        {
            deleteTax.Isdeleted = true;
            deleteTax.Updatedby = userId;
            deleteTax.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
        }  
    }
    
    public async Task<bool> IsTaxExist(string name)
    {
        var tnf = await _context.Taxesandfees.Where(t => t.Isdeleted == false && t.Name.ToLower() == name.ToLower()).ToListAsync();
        int count = tnf.Count();
        if (count > 0)
            return true;
        return false;
    }

    public async Task<bool> IsEditTaxExist(string name, int? id)
    {
        var tax = await _context.Taxesandfees.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower()).ToListAsync();
        foreach (var m in tax)
        {
            if (m.Id != id)
                return true;
        }
        return false;
    }
    
}
