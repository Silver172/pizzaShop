using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;

    public OrderRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrderList(PaginationViewModel pagination)
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
            return await _context.Orders.Where(o => o.Isdeleted == false &&
                (string.IsNullOrEmpty(pagination.SearchString) || o.Id.ToString().Contains(pagination.SearchString) || o.Customer.Name.ToLower().Contains(pagination.SearchString.ToLower())) &&
                (pagination.Status == "All Status" || o.Status == pagination.Status) &&
                (pagination.Time == "All Time" || (o.Createddate >= startDate && o.Createddate <= endDate)) &&
                (pagination.ToDate == null || pagination.FromDate == null || (o.Createddate >= pagination.FromDate && o.Createddate <= pagination.ToDate)))
                .OrderBy<Order, object>(o => pagination.SortingBy == "Customer" ? o.Customer.Name : pagination.SortingBy == "Date" ? o.Createddate : pagination.SortingBy == "TotalAmount" ? o.Totalamount : o.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();
        }
        else
        {
            return await _context.Orders.Where(o => o.Isdeleted == false &&
                (string.IsNullOrEmpty(pagination.SearchString) || o.Id.ToString().Contains(pagination.SearchString) || o.Customer.Name.ToLower().Contains(pagination.SearchString.ToLower())) &&
                (pagination.Status == "All Status" || o.Status == pagination.Status) &&
                (pagination.Time == "All Time" || (o.Createddate >= startDate && o.Createddate <= endDate)) &&
                (pagination.ToDate == null || pagination.FromDate == null || (o.Createddate >= pagination.FromDate && o.Createddate <= pagination.ToDate)))
                .OrderByDescending<Order, object>(o => pagination.SortingBy == "Customer" ? o.Customer.Name : pagination.SortingBy == "Date" ? o.Createddate : pagination.SortingBy == "TotalAmount" ? o.Totalamount : o.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();
        }

    }

    public async Task<List<Order>> GetOrdersCount(PaginationViewModel pagination)
    {
        DateTime endDate = DateTime.Now;
        var startDate = pagination.Time switch
        {
            "Last 7 days" => DateTime.Now.AddDays(-7),
            "Last 30 days" => DateTime.Now.AddDays(-30),
            "Current Month" => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
            _ => DateTime.MinValue,
        };

        return await _context.Orders.Where(o => o.Isdeleted == false &&
        (string.IsNullOrEmpty(pagination.SearchString) || o.Id.ToString().Contains(pagination.SearchString) || o.Customer.Name.ToLower().Contains(pagination.SearchString.ToLower())) &&
        (pagination.Status == "All Status" || o.Status == pagination.Status) &&
        (pagination.Time == "All Time" || (o.Createddate >= startDate && o.Createddate <= endDate)) &&
        (pagination.ToDate == null || pagination.FromDate == null || (o.Createddate >= pagination.FromDate && o.Createddate <= pagination.ToDate)))
        .ToListAsync();
    }

    public async Task<Order> OrderDetails(int id)
    {
        var orderDetails = await _context.Orders.Where(o => o.Id == id)
                                    .Include(i => i.Customer)
                                    .Include(o => o.Invoices)
                                    .Include(o => o.Tableordermappings)
                                    .ThenInclude(o => o.Table)
                                    .ThenInclude(o => o.Section)
                                    .Include(o => o.Orderitems.Where(o => o.Isdeleted == false))
                                    .ThenInclude(O => O.Orderitemmodifiermappings)
                                    .ThenInclude(o => o.Modifier)
                                    .Include(o => o.Payment)
                                    .Include(o => o.Ordertaxmappings)
                                    .ThenInclude(o => o.Tax)
                                    .FirstOrDefaultAsync();
        return orderDetails!;
    }

    public async Task<List<Order>> GetKotContent(int categoryId, PaginationViewModel pagination)
    {
        string filterString = pagination.FilterBy ?? "In Progress";
        var orderDetails = await _context.Orders.Where(o => o.Status == "In Progress" && o.Isdeleted == false && o.Orderitems.Any(item => ((filterString == "In Progress" && item.Quantity > item.Readyitemquantity) || (filterString == "Ready" && item.Readyitemquantity > 0)) && (item.Menuitem.Categoryid == categoryId || categoryId == 0) && o.Isdeleted == false))
                                    .Include(o => o.Tableordermappings)
                                    .ThenInclude(o => o.Table)
                                    .ThenInclude(o => o.Section)
                                    .Include(o => o.Orderitems.Where(o => ((filterString == "In Progress" && o.Quantity > o.Readyitemquantity) || filterString == "Ready" && o.Readyitemquantity > 0) && (o.Menuitem.Categoryid == categoryId || categoryId == 0) && o.Isdeleted == false))
                                    .ThenInclude(O => O.Orderitemmodifiermappings)
                                    .ThenInclude(o => o.Modifier)
                                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();
        return orderDetails;
    }

    public async Task<int> GetKotContentCount(int categoryId, PaginationViewModel pagination)
    {
        string filterString = pagination.FilterBy ?? "In Progress";
        int orderDetails = await _context.Orders.Where(o => o.Status == "In Progress" && o.Isdeleted == false && o.Orderitems.Any(item => ((filterString == "In Progress" && item.Quantity > item.Readyitemquantity) || (filterString == "Ready" && item.Readyitemquantity > 0)) && (item.Menuitem.Categoryid == categoryId || categoryId == 0) && o.Isdeleted == false))
                                    .Include(o => o.Tableordermappings)
                                    .ThenInclude(o => o.Table)
                                    .ThenInclude(o => o.Section)
                                    .Include(o => o.Orderitems.Where(o => ((filterString == "In Progress" && o.Quantity > o.Readyitemquantity) || filterString == "Ready" && o.Readyitemquantity > 0) && (o.Menuitem.Categoryid == categoryId || categoryId == 0) && o.Isdeleted == false))
                                    .ThenInclude(O => O.Orderitemmodifiermappings)
                                    .ThenInclude(o => o.Modifier)
                                    .CountAsync();
        return orderDetails;
    }

    public async Task<Order> creteOrder(AddEditWaitingTokenViewModel model, string userId)
    {
        Order order = new Order
        {
            Customerid = model.CustomerId,
            Subtotalamount = 0,
            Totalamount = 0,
            Tax = 0,
            Discount = 0,
            Notes = "",
            Status = "Pending",
            Isgstselected = false,
            Isdeleted = false,
            Createddate = DateTime.Now,
            Createdby = userId
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> UpdateOrder(OrderDetailViewModel order, string userId)
    {
        Order updatedOrder = await _context.Orders.FirstOrDefaultAsync(i => i.Id == order.Id && i.Isdeleted == false);

        updatedOrder.Subtotalamount = order.SubtotalAmount;
        updatedOrder.Totalamount = order.TotalAmount;
        updatedOrder.Tax = order.TotalAmount - order.SubtotalAmount;
        updatedOrder.Discount = 0;
        updatedOrder.Status = "In Progress";
        updatedOrder.Isgstselected = order.IsSGSTSeclected;
        updatedOrder.Updateddate = DateTime.Now;
        updatedOrder.Updatedby = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddEditOrderComment(int id, string comments, string userId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == id && i.Isdeleted == false);
        if (order != null)
        {
            order.Notes = comments.Trim();
            order.Updatedby = userId;
            order.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task UpdateOrderStatus(int id, string status, string userId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == id && i.Isdeleted == false);
        if (order != null)
        {
            order.Status = status;
            order.Updatedby = userId;
            order.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Order>> GetOrdersByCustomerId(int id)
    {
        List<Order> orders = await _context.Orders.Where(o => o.Customerid == id).ToListAsync();
        return orders;
    }

     public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
    public async Task<DashboardViewModel> GetDashboardData(string? TimePeriod)
    {
        DateTime fromDate = DateTime.Now;
        DateTime toDate = DateTime.Now;

        if(TimePeriod == "Current Month")
        {
            fromDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
            toDate = fromDate.AddMonths(1).AddDays(-1);
        }
        int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        
        if(TimePeriod == "This Week")
        {
            fromDate = StartOfWeek(DateTime.Now, DayOfWeek.Monday);
            toDate = fromDate.AddDays(7);
        }
        else if(TimePeriod == "This Year")
        {
            fromDate = new DateTime(DateTime.Now.Year,1,1);
            toDate = new DateTime(DateTime.Now.Year, 12, 31);

        }else if(TimePeriod == "Today")
        {
            fromDate = DateTime.Now.Date;
            toDate = fromDate.AddDays(1);
        }else if(TimePeriod == "Last 30 days")
        {
            toDate = DateTime.Now.Date;
            fromDate = toDate.AddDays(-30);
        }

        List<Order> orders = await _context.Orders.Where(o => o.Createddate >= fromDate && o.Createddate <= toDate && o.Isdeleted == false && o.Status != "Cancelled").ToListAsync();
        DashboardViewModel DashboardPage = new DashboardViewModel{
            TotalOrders = orders.Count(),
            TotalSales = Math.Round((decimal)orders.Sum(o => o.Totalamount) , 2),
            AverageOrderValue = orders.Count()> 0 ?  Math.Round((decimal) orders.Average(o => o.Totalamount) , 2) : 0,
            AverageWaitingTime = orders.Count() > 0 ? Math.Round(orders.Where(o => o.Updateddate != null).Average(o => (o.Updateddate - o.Createddate)?.TotalMinutes ?? 0 ),2) : 0,
            TopSellingItem = await _context.Orderitems.Where(i => i.Order.Createddate >= fromDate && i.Order.Createddate <= toDate && i.Order.Status != "Cancelled")
                            .GroupBy(oi => new { oi.Menuitem.Id , oi.Menuitem.Name, oi.Menuitem.Itemimage})
                            .Select(i => new DashboardItemViewModel{
                                Id = i.Key.Id,
                                Name = i.Key.Name,
                                ItemImage = i.Key.Itemimage,
                                OrderCount = i.Sum(oi => oi.Quantity)
                            })
                            .OrderByDescending(g => g.OrderCount)
                            .Take(2)
                            .ToListAsync()
            ,
            LeastSellingItem = await _context.Orderitems.Where(i => i.Order.Createddate >= fromDate && i.Order.Createddate <= toDate && i.Order.Status != "Cancelled")
                            .GroupBy(oi => new {oi.Menuitem.Id , oi.Menuitem.Name, oi.Menuitem.Itemimage})
                            .Select(i => new DashboardItemViewModel{
                                Id = i.Key.Id,
                                Name = i.Key.Name,
                                ItemImage = i.Key.Itemimage,
                                OrderCount = i.Sum(oi => oi.Quantity)
                            })
                            .OrderBy(o => o.OrderCount)
                            .Take(2)
                            .ToListAsync()
            ,
            NewCustomer = await _context.Customers.CountAsync(c => c.Createddate >= fromDate && c.Createddate <= toDate),
            WaitingListCount = await _context.Waitingtokens.CountAsync(w => w.Createddate >= fromDate && w.Createddate <= toDate && w.Isdeleted == false),
            Revenue = new GraphDetailViewModel
            {
                Labels = TimePeriod switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth).Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString("dd")).ToList(),
                    "This Week" => Enumerable.Range(0, 7).Select(day => fromDate.AddDays(day).ToString("dddd")).ToList(),
                    "This Year" => Enumerable.Range(1, 12).Select(month => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM")).ToList(),
                    "Today" => Enumerable.Range(0, 12).Select(hour => $"{9 + hour}:00 - {9 + hour + 1}:00").ToList(),
                    "Last 30 days" => Enumerable.Range(0, 30).Select(day => fromDate.AddDays(day).ToString("MM/dd")).ToList(),
                    _ => new List<string>()
                },
                Values = TimePeriod switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth) //outer sequence
                    .Select(day => fromDate.AddDays(day - 1))
                    .GroupJoin(
                        orders, // inner sequence
                        date => date.Date, // dd/dd/dddd 02:67:6767   outer sequence key selector
                        order => order.Createddate.HasValue ? order.Createddate.Value.Date : DateTime.MinValue.Date, //inner sequence key selector
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.Totalamount), 2)
                    )
                    .ToList(),
                    "This Week" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        orders,
                        date => date.Date,
                        order => order.Createddate.HasValue ? order.Createddate.Value.Date : DateTime.MinValue.Date,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.Totalamount), 2)
                    )
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1))
                    .GroupJoin(
                        orders,
                        date => date.Month,
                        order => order.Createddate.HasValue ? order.Createddate.Value.Month : 0,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.Totalamount), 2)
                    )
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => fromDate.AddHours(8 + hour))
                    .GroupJoin(
                        orders,
                        date => date.Hour,
                        order => order.Createddate.HasValue ? order.Createddate.Value.Hour : 0,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.Totalamount), 2)
                    )
                    .ToList(),
                    "Last 30 days" => Enumerable.Range(0, 30)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        orders,
                        date => date.Date,
                        order => order.Createddate.HasValue ? order.Createddate.Value.Date : DateTime.MinValue.Date,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.Totalamount), 2)
                    )
                    .ToList(),
                    _ => new List<decimal>()
                }
            },
            CustomerGrowth = new GraphDetailViewModel
            {
                Labels = TimePeriod switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth).Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString("dd")).ToList(),
                    "This Week" => Enumerable.Range(0, 7).Select(day => fromDate.AddDays(day).ToString("dddd")).ToList(),
                    "This Year" => Enumerable.Range(1, 12).Select(month => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM")).ToList(),
                    "Today" => Enumerable.Range(0, 12).Select(hour => $"{9 + hour}:00 - {9 + hour + 1}:00").ToList(),
                    "Last 30 days" => Enumerable.Range(0, 30).Select(day => fromDate.AddDays(day).ToString("MM/dd")).ToList(),
                    _ => new List<string>()
                },
                Values = TimePeriod switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth)
                    .Select(day => fromDate.AddDays(day - 1))
                    .GroupJoin(
                        orders,
                        date => date.Date,
                        customer => customer.Createddate.HasValue ? customer.Createddate.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "This Week" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        orders,
                        date => date.Date,
                        customer => customer.Createddate.HasValue ? customer.Createddate.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1))
                    .GroupJoin(
                        orders,
                        date => date.Month,
                        customer => customer.Createddate.HasValue ? customer.Createddate.Value.Month : 0,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => fromDate.AddHours(8 + hour))
                    .GroupJoin(
                        orders,
                        date => date.Hour,
                        customer => customer.Createddate.HasValue ? customer.Createddate.Value.Hour : 0,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "Last 30 days" => Enumerable.Range(1, 30)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        orders,
                        date => date.Date,
                        customer => customer.Createddate.HasValue ? customer.Createddate.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    _ => new List<decimal>()
                }
            }
        };

        return DashboardPage;
    }

}