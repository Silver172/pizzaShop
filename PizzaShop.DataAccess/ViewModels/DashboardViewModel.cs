namespace PizzaShop.DataAccess.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public decimal TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public double? AverageWaitingTime { get; set; }
    public GraphDetailViewModel? Revenue { get; set; }
    public GraphDetailViewModel? CustomerGrowth { get; set; }
    public List<DashboardItemViewModel>? TopSellingItem { get; set; }
    public List<DashboardItemViewModel>? LeastSellingItem { get; set; }
    public int WaitingListCount { get; set; }
    public int NewCustomer { get; set; }
}
