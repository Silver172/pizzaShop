using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IDashboardService
{
    public Task<DashboardViewModel> GetDashboardPage(string? TimePeriod = "Current Month");
}
