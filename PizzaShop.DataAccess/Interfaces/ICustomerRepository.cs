using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ICustomerRepository
{
    public Task<Customer?> GetCustomerById(int? id);
    public Task<List<Customer>> GetCustomers(PaginationViewModel pagination);
    public Task<List<Customer>> GetCustomersCount(PaginationViewModel pagination);
    public Task<Customer> GetCustomerHistory(int id);
    public Task<Customer>? IsCustomerExist(string email);
    public Task<Customer> AddNewCustomer(string name, string email, string phone, string userId);
    public Task<Customer> EditCustomer(string name, string email, string phone, string userId);
    public Task<List<Customer>> SearchCustomer(string? searchString);
}
