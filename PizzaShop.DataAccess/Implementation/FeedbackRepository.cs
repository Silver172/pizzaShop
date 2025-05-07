using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly PizzashopContext _context;

    public FeedbackRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetFeedbackByOrderId(int? id)
    {
        var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Orderid == id);
        if (feedback != null)
            return feedback;
        return null;
    }

    public async Task SaveCustomerFeedback(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
        await _context.SaveChangesAsync();
    }
}
