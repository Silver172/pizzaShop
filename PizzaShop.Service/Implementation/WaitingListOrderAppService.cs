using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class WaitingListOrderAppService : IWaitingListOrderAppService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IWaitingTokenRepository _waitingRepository;

    private readonly ICustomerRepository _customerRepository;
    private readonly ITableRepository _tableRepository;
    public WaitingListOrderAppService(ISectionRepository sectionRepository, IWaitingTokenRepository waitingRepository, ICustomerRepository customerRepository, ITableRepository tableRepository)
    {
        _sectionRepository = sectionRepository;
        _waitingRepository = waitingRepository;
        _customerRepository = customerRepository;
        _tableRepository = tableRepository;
    }

    public async Task<WaitingListOrderAppViewModel> GetWaitingListPage(int? id)
    {
        WaitingListOrderAppViewModel waitingListPage = new WaitingListOrderAppViewModel();
        List<Section> sections = await _sectionRepository.GetSectionsForWaitingList();
        List<Waitingtoken> waitingTokens = await _waitingRepository.GetWaitingTokensBySectionId(id);
        List<SectionViewModel> sectionList = new List<SectionViewModel>();
        List<WaitingListViewModel> waitingList = new List<WaitingListViewModel>();

        foreach (var s in sections)
        {
            sectionList.Add(new SectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                waitingTokenCount = s.Waitingtokens.Where(w => w.Isdeleted == false && w.Isassigned == false).Count()
            });
        }

        waitingListPage.SectionList = sectionList;

        foreach (var w in waitingTokens)
        {
            string createdDate = w.Createddate.HasValue ? w.Createddate.Value.ToString("MMMM dd yyyy h:mm tt") : string.Empty;
            string waitingTime = w.Createddate.HasValue 
                ? DateTime.Now.Subtract(w.Createddate.Value).Days + " days " + DateTime.Now.Subtract(w.Createddate.Value).Hours + " hrs " + DateTime.Now.Subtract(w.Createddate.Value).Minutes + " min" 
                : "0 min";
            waitingList.Add(new WaitingListViewModel
            {
                Id = w.Id,
                CreatedDate = createdDate,
                Email = w.Customer.Email,
                Phone = w.Customer.Phone,
                CustomerName = w.Customer.Name,
                IsAssigned = w.Isassigned,
                NoOfPersons = w.Noofpersons,
                SectionId = w.Sectionid,
                WaitingTime = waitingTime
            });
        }

        waitingListPage.WaitingList = waitingList;
        return waitingListPage;
    }

    public async Task<List<SectionViewModel>> GetSectionList()
    {
        List<Section> sections = await _sectionRepository.GetSectionsForWaitingList();
        List<SectionViewModel> sectionList = new List<SectionViewModel>();

        foreach (var s in sections)
        {
            sectionList.Add(new SectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                waitingTokenCount = s.Waitingtokens.Where(w => w.Isdeleted == false && w.Isassigned == false).Count()
            });
        }
        return sectionList;
    }

    public async Task<AddEditWaitingTokenViewModel> GetAddEditWaitingToken(int? id)
    {
        AddEditWaitingTokenViewModel waitingToken = new AddEditWaitingTokenViewModel();
        List<SectionViewModel> sections = await _sectionRepository.GetAllSections();
        waitingToken.SectionList = sections;
        if (id == null || id == 0)
        {
            return waitingToken;
        }
        else
        {
            Waitingtoken Wt = await _waitingRepository.GetWaitingTokeById(id);

            waitingToken.Id = Wt.Id;
            waitingToken.Email = Wt.Customer.Email;
            waitingToken.Name = Wt.Customer.Name;
            waitingToken.MobileNumber = Wt.Customer.Phone;
            waitingToken.CustomerId = Wt.Customer.Id;
            waitingToken.SectionId = Wt.Sectionid;
            waitingToken.NoOfPersons = Wt.Noofpersons;
            return waitingToken;
        }
    }

    public async Task<bool> AddEditWaitingToken(AddEditWaitingTokenViewModel model, string userId)
    {
        if (model.Id == null || model.Id == 0)
        {
            Customer isExist = await _customerRepository.IsCustomerExist(model.Email);
            Customer customer;
            if (isExist != null)
                customer = await _customerRepository.EditCustomer(model.Name, model.Email, model.MobileNumber, userId);
            else
                customer = await _customerRepository.AddNewCustomer(model.Name, model.Email, model.MobileNumber, userId);
            model.CustomerId = customer.Id;
            await _waitingRepository.AddWaitingToken(model, userId);
            return true;
        }
        else
        {
            await _customerRepository.EditCustomer(model.Name, model.Email, model.MobileNumber, userId);
            await _waitingRepository.EditWaitingToken(model, userId);
            return true;
        }
    }

    public async Task<bool> DeleteWaitingtoken(int id,string userId)
    {
        await _waitingRepository.DeleteWaitingToken(id,userId);
        return true;
    }

    public  async Task<List<CustomerSuggestionList>> SearchCustomerByEmail(string? email)
    {
        List<Customer> customerList = await _customerRepository.SearchCustomer(email);
        List<CustomerSuggestionList> csList = new List<CustomerSuggestionList>();
        
        foreach (var c in customerList)
        {
            csList.Add(new CustomerSuggestionList{
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            });
        }
        return csList;
    }

    public async Task<List<Section>> AssignWaitingTokenToTable(int id)
    {
        List<Section> sections = await _sectionRepository.GetSectionsForWaitingList();
        return sections;
    }

    public async Task<JsonArray> GetTablesBySectionId(int id)
    {
        List<Table> tables = await _tableRepository.GetAvailableTablesBySectionId(id);
        JsonArray TableList = new JsonArray();
        foreach (var t in tables)
        {
            JsonObject table = new JsonObject
            {
                ["Id"] = t.Id,
                ["Name"] = t.Name,
                ["Capacity"] = t.Capacity,
                ["Status"] = t.Status,
                ["IsAvailable"] = t.Isavailable
            };
            TableList.Add(table);
        }
        return TableList;
    }
}
