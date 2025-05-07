using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Scaffolding;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class TableOrderAppService : ITableOrderAppService
{
    public readonly ISectionRepository _sectionRepository;
    public readonly ICustomerRepository _customerRepository;
    public readonly IWaitingTokenRepository _waitingTokenRepository;
    public readonly ITableRepository _tableRepository;
    public readonly IOrderRepository _orderRepository;
    public readonly ITableOrderMappingRepository _tableOrderMappingRepository;
    public TableOrderAppService(ISectionRepository sectionRepository, ICustomerRepository customerRepository, IWaitingTokenRepository waitingRepository, ITableRepository tableRepository, IOrderRepository orderRepository, ITableOrderMappingRepository tableOrderMappingRepository)
    {
        _sectionRepository = sectionRepository;
        _customerRepository = customerRepository;
        _waitingTokenRepository = waitingRepository;
        _tableRepository = tableRepository;
        _orderRepository = orderRepository;
        _tableOrderMappingRepository = tableOrderMappingRepository;
    }

    public async Task<TableViewOrderAppViewModel> GetTableView()
    {
        var tableView = new TableViewOrderAppViewModel();

        var section = await _sectionRepository.GetTableViewOrderApp();
        tableView.SectionList = section.Select(s => new SectionOrderAppViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Available = s.Tables.Where(t => t.Isdeleted == false && t.Status == "Available").Count(),
            Assigned = s.Tables.Where(t => t.Isdeleted == false && t.Status == "Occupied")
                .SelectMany(t => t.Tableordermappings)
                .Count(m => m.Order.Status == "Pending"),
            Running = s.Tables.Where(t => t.Isdeleted == false && t.Status == "Occupied")
                .SelectMany(t => t.Tableordermappings)
                .Count(m => m.Order.Status == "In Progress" || m.Order.Status == "Served"),
            TableList = s.Tables.Where(t => t.Isdeleted == false).Select(t => new TableOrderAppViewModel
            {
                Id = t.Id,
                Sectionid = t.Sectionid,
                Name = t.Name,
                Capacity = t.Capacity,
                Status = GetTableStatus(t),
                TotalAmount = t.Tableordermappings.Select(t => (t.Order.Status == "In Progress" || t.Order.Status == "Pending") ? true : false).LastOrDefault() ? t.Tableordermappings.Select(t => t.Order.Totalamount).LastOrDefault() : (decimal?)00.00,
                TimeDuration = (t.Tableordermappings.Select(o => o.Createddate).LastOrDefault() != null && t.Status == "Occupied" &&
                                t.Tableordermappings.Any(s => s.Order.Status == "Pending" || s.Order.Status == "In Progress" || s.Order.Status == "Served"))
                    ? (DateTime.Now - t.Tableordermappings.Select(o => o.Createddate).LastOrDefault().Value).Days.ToString() + " Days," +
                     (DateTime.Now - t.Tableordermappings.Select(o => o.Createddate).LastOrDefault().Value).Hours.ToString() + " Hours,"
                     + (DateTime.Now - t.Tableordermappings.Select(o => o.Createddate).LastOrDefault().Value).Minutes.ToString() + " Minutes"
                        : "0 Mins",
                // TimeDuration = DateTime.Now.Subtract((DateTime)t.Createddate).Days.ToString() + " Days " + DateTime.Now.Subtract((DateTime)t.Createddate).Hours.ToString() + " hours "+  DateTime.Now.Subtract((DateTime)t.Createddate).Minutes.ToString() + " min " + DateTime.Now.Subtract((DateTime)t.Createddate).Seconds.ToString() + " sec ",
                OrderStatus = t.Tableordermappings.Select(t => t.Order?.Status).LastOrDefault(),
                OrderId = t.Tableordermappings.Select(t => t.Order?.Id).LastOrDefault(),
            }).ToList()
        }).ToList();

        return tableView;
    }

    private string GetTableStatus(Table t){
        string orderStatus = t.Tableordermappings.Select(t => t.Order?.Status).LastOrDefault();
        string status = ((orderStatus == "In Progress" || orderStatus == "Served") && t.Status == "Occupied") ? "table-running" : (orderStatus == "Pending" && t.Status == "Occupied") ? "table-assigned" : "table-available";
        return status;
    }
    
    public async Task<AddEditWaitingTokenViewModel> AssignTableOffcanvasData(int? id)
    {
        var data = new AddEditWaitingTokenViewModel();
        var waitingTokens = await _waitingTokenRepository.GetWaitingTokensBySectionId(id);
        List<SectionViewModel> sections = await _sectionRepository.GetAllSections();
        data.SectionList = sections;

        data.WaitingList = waitingTokens.Select(w => new WaitingListViewModel
        {
            Id = w.Id,
            CreatedDate = w.Createddate.HasValue ? w.Createddate.Value.ToString("MMMM dd yyyy h:mm tt") : string.Empty,
            Email = w.Customer.Email,
            Phone = w.Customer.Phone,
            CustomerName = w.Customer.Name,
            IsAssigned = w.Isassigned,
            NoOfPersons = w.Noofpersons,
            SectionId = w.Sectionid,
            WaitingTime = w.Createddate.HasValue
                ? DateTime.Now.Subtract(w.Createddate.Value).Hours + " hrs " + DateTime.Now.Subtract(w.Createddate.Value).Minutes + " min"
                : "0 min"
        }).ToList();

        data.SectionId = id;

        return data;
    }

    public async Task<int> AssignTable(AddEditWaitingTokenViewModel model, string userId, List<int> tableIds)
    {
        //create or update customer
        Customer isExist = await _customerRepository.IsCustomerExist(model.Email);
        Customer customer;
        if (isExist != null)
        {   
            List<Order> orderList = await _orderRepository.GetOrdersByCustomerId(isExist.Id); 
            bool anyOrderRunning = orderList.Any(o => o.Status == "In Progress" || o.Status == "Pending");
            if(anyOrderRunning)
                return -1;
            customer = await _customerRepository.EditCustomer(model.Name, model.Email, model.MobileNumber, userId);
        }
        else
            customer = await _customerRepository.AddNewCustomer(model.Name, model.Email, model.MobileNumber, userId);
        model.CustomerId = customer.Id;

        if(model.Id != null && model.Id != 0)
        {
            model.IsAssigned = true;
            model.IsDeleted = false;
            await _waitingTokenRepository.EditWaitingToken(model, userId);
        }

        // create order and update table status
        Order order = await _orderRepository.creteOrder(model, userId);
        foreach (var t in tableIds)
        {
            await _tableOrderMappingRepository.AddMapping(model, t, order.Id, userId);
            await _tableRepository.UpdateTableStatus(t, "Occupied", userId);
        }
        
        
        return order.Id;
    }


    public async Task<List<Table>> GetOptimizedTables(AddEditWaitingTokenViewModel model, List<int> tableIds)
    {
        List<Table> tableList = await _tableRepository.GetOptimizedTablesBySectionId(model.SectionId, model.NoOfPersons, tableIds);
        return tableList;
    }

}
