using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class PaymentRepository: IPaymentRepository
{
    private readonly PizzashopContext _context;

    public PaymentRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetPaymentById(int? id)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddPayment(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }
}
