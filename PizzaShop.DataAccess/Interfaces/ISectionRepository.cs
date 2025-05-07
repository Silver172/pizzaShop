using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ISectionRepository
{
    public Task<List<SectionViewModel>> GetAllSections();
    public Task AddSection(AddEditSectionViewModel model,string userId);
    public Task<Section?> GetSectionById(int? id);
    public Task EditSection(AddEditSectionViewModel model,string userId);
    public Task DeleteSection(int id,string userId);
    public Task<bool> IsSectionExist(string name);
    public Task<bool> isEditSectionExist(string name,int? id);
    public Task<List<Section>> GetTableViewOrderApp();
    public Task<List<Section>> GetSectionsForWaitingList();
    public Task<List<Section>> getSections();
}
