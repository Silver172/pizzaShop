using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class InvoiceRepository: IInvoiceRepository
{
    private readonly PizzashopContext _context;

    public InvoiceRepository(PizzashopContext context)
    {
        _context = context;
    }


    public async Task SaveInvoiceDetail(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }
}
