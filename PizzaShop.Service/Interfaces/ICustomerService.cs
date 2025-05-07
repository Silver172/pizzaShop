using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    public Task<CustomerPageViewModel> GetCustomerPage();
    public Task<CustomerPageViewModel> GetFilteredCustomer(PaginationViewModel pagination);
    public Task<FileContentResult> ExportCustomersExcel(PaginationViewModel pagination);
    public Task<CustomerHistoryViewModel> GetCustomerHistory(int id);
}
