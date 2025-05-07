using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IFeedbackRepository
{
    public Task<Feedback?> GetFeedbackByOrderId(int? id);
    public Task SaveCustomerFeedback(Feedback feedback);
}
