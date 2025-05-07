using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class CustomerRepository : ICustomerRepository
{
    private readonly PizzashopContext _context;

    public CustomerRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetCustomerById(int? id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Customer>> GetCustomers(PaginationViewModel pagination)
    {
        DateTime endDate = DateTime.Now;
        var startDate = pagination.Time switch
        {
            "Last 7 days" => DateTime.Now.AddDays(-7),
            "Last 30 days" => DateTime.Now.AddDays(-30),
            "Current Month" => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
            _ => DateTime.MinValue,
        };

        if (pagination.SortingType == "ascending")
        {
            return await _context.Customers.Where(c =>
                (string.IsNullOrEmpty(pagination.SearchString) || c.Name.ToLower().Contains(pagination.SearchString.Trim().ToLower())) &&
                (pagination.Time == "All Time" || (c.Createddate >= startDate && c.Createddate <= endDate)) &&
                (pagination.ToDate == null || pagination.FromDate == null || (c.Createddate >= pagination.FromDate && c.Createddate <= pagination.ToDate))
                )
                .OrderBy<Customer, object>(c => pagination.SortingBy == "Date" ? c.Createddate : pagination.SortingBy == "TotalOrder" ? c.Orders.Count() : c.Name)
                    .Include(c => c.Orders)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();
        }
        else
        {
            return await _context.Customers.Where(c =>
                (string.IsNullOrEmpty(pagination.SearchString) || c.Name.ToLower().Contains(pagination.SearchString.ToLower())) &&
                (pagination.Time == "All Time" || (c.Createddate >= startDate && c.Createddate <= endDate)) &&
                (pagination.ToDate == null || pagination.FromDate == null || (c.Createddate >= pagination.FromDate && c.Createddate <= pagination.ToDate))
                )
                .OrderByDescending<Customer, object>(c => pagination.SortingBy == "Date" ? c.Createddate : pagination.SortingBy == "TotalOrder" ? c.Orders.Count() : c.Name)
                    .Include(c => c.Orders)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();
        }
    }

    public async Task<List<Customer>> GetCustomersCount(PaginationViewModel pagination)
    {
        DateTime endDate = DateTime.Now;
        var startDate = pagination.Time switch
        {
            "Last 7 days" => DateTime.Now.AddDays(-7),
            "Last 30 days" => DateTime.Now.AddDays(-30),
            "Current Month" => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
            _ => DateTime.MinValue,
        };

        return await _context.Customers.Where(c =>
                (string.IsNullOrEmpty(pagination.SearchString) || c.Name.ToLower().Contains(pagination.SearchString.ToLower())) &&
                (pagination.Time == "All Time" || (c.Createddate >= startDate && c.Createddate <= endDate)) &&
                (pagination.ToDate == null || pagination.FromDate == null || (c.Createddate >= pagination.FromDate && c.Createddate <= pagination.ToDate))
                )
                .OrderBy<Customer, object>(c => pagination.SortingBy == "Date" ? c.Name : pagination.SortingBy == "TotalOrder" ? c.Createddate : c.Name)
                    .Include(c => c.Orders)
                    .ToListAsync();
    }

    public async Task<Customer> GetCustomerHistory(int id)
    {
        var customerHistory = await _context.Customers.Where(c => c.Id == id)
                                    .Include(c => c.Orders)
                                    .ThenInclude(o => o.Payment)
                                    .Include(o => o.Orders)
                                    .ThenInclude(o => o.Orderitems)
                                    .FirstOrDefaultAsync();
        return customerHistory;
    }

    public async Task<Customer>? IsCustomerExist(string email)
    {
        Customer? customer = await _context.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
        if (customer?.Id != null)
            return customer;
        else
            return null;
    }

    public async Task<Customer> AddNewCustomer(string name, string email, string phone, string userId)
    {
        Customer newCustomer = new Customer
        {
            Name = name,
            Email = email,
            Phone = phone,
            Createdby = userId,
            Createddate = DateTime.Now
        };
        await _context.Customers.AddAsync(newCustomer);
        await _context.SaveChangesAsync();
        return newCustomer;
    }

    public async Task<Customer> EditCustomer(string name, string email, string phone, string userId)
    {

        Customer editCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

        editCustomer.Name = name;
        editCustomer.Phone = phone;
        editCustomer.Updatedby = userId;
        editCustomer.Createddate = DateTime.Now;

        await _context.SaveChangesAsync();
        return editCustomer;
    }

    public async Task<List<Customer>> SearchCustomer(string? searchString)
    {
        List<Customer> customerList = new List<Customer>();

        if (!string.IsNullOrEmpty(searchString))
        {
            customerList = await _context.Customers.Where(c => c.Email.ToLower().Contains(searchString.Trim().ToLower()))
            .ToListAsync();
        }
        return customerList;
    }
}
