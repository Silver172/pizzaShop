using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    public Task<OrdersPageViewModel> GetOrders(PaginationViewModel pagination);
    public Task<OrdersPageViewModel> GetOrdersWithoutPagination(PaginationViewModel pagination);
    public Task<FileContentResult> ExportOrderExcel(PaginationViewModel pagination);
    public Task<OrderDetailViewModel> OrderDetails(int id);
}
