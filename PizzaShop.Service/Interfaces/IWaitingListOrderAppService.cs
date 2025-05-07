using System.Text.Json.Nodes;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IWaitingListOrderAppService
{
    public Task<AddEditWaitingTokenViewModel> GetAddEditWaitingToken(int? id);
    public Task<WaitingListOrderAppViewModel> GetWaitingListPage(int? id);
    public Task<bool> AddEditWaitingToken(AddEditWaitingTokenViewModel model, string userId);
    public Task<List<SectionViewModel>> GetSectionList();
    public Task<bool> DeleteWaitingtoken(int id,string userId);
    public  Task<List<CustomerSuggestionList>> SearchCustomerByEmail(string? email);
    public Task<JsonArray> GetTablesBySectionId(int id);
}
