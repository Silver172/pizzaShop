using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IInvoiceRepository
{
    public Task SaveInvoiceDetail(Invoice invoice);
}
