using System.Threading.Tasks;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;

    public DashboardService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<DashboardViewModel> GetDashboardPage(string? TimePeriod)
    {
        return await _orderRepository.GetDashboardData(TimePeriod);
    }
}
