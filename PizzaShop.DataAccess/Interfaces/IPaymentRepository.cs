using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IPaymentRepository
{
     public Task<Payment?> GetPaymentById(int? id);
     public Task AddPayment(Payment payment);
}
