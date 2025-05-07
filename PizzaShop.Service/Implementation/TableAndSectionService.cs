using System.Threading.Tasks;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class TableAndSectionService : ITableAndSectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ITableRepository _tableRepository;

    public TableAndSectionService(ISectionRepository sectionrepository, ITableRepository tableRepository)
    {
        _sectionRepository = sectionrepository;
        _tableRepository = tableRepository;
    }

    public async Task<TableAndSectionViewModel> GetTableSectionPage()
    {
        var pagination = new PaginationViewModel
        {
            PageIndex = 1,
            PageSize = 5,
            SearchString = "",
        };

        var tableSectionPage = new TableAndSectionViewModel();
        var sectionList = await _sectionRepository.GetAllSections();
        var tableList = new List<TableViewModel>();
        if (sectionList.Count > 0)
        {
            tableList = await _tableRepository.GetTablesBySectionId(sectionList.First().Id, pagination);
            int tr = await _tableRepository.GetTablesCount(sectionList.First().Id,pagination.SearchString);
            pagination.TotalRecord = tr;
            pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);
        }


        tableSectionPage.Pagination = pagination;
        tableSectionPage.TableList = tableList;
        tableSectionPage.SectionList = sectionList;
        return tableSectionPage;
    }

    public async Task<List<SectionViewModel>> GetAllSection()
    {
       return await _sectionRepository.GetAllSections();
    }

    public async Task<List<TableViewModel>> GetTablesBySectionId(int id, PaginationViewModel pagination)
    {
        var tableList = await _tableRepository.GetTablesBySectionId(id, pagination);
        return tableList;
    }

    public async Task<TableAndSectionViewModel?> AddSection(AddEditSectionViewModel model,string userId)
    {
        bool isExist = await _sectionRepository.IsSectionExist(model.Name);
        if(isExist)
            return null;
        await _sectionRepository.AddSection(model,userId);
        var TSPage = await GetTableSectionPage();
        return TSPage;
    }

    public async Task<AddEditSectionViewModel> GetSectionById(int id)
    {
        var section = await _sectionRepository.GetSectionById(id);
        var editSection = new AddEditSectionViewModel{
            Id = section.Id,
            Name = section.Name,
            Description = section.Description
        };
        return editSection;
    }

    public async Task<TableAndSectionViewModel?> EditSection(AddEditSectionViewModel model,string userId)
    {
        bool isExist = await _sectionRepository.isEditSectionExist(model.Name,model.Id);
        if(isExist)
            return null;
        await _sectionRepository.EditSection(model,userId);
        var TSPage = await GetTableSectionPage();
        return TSPage;
    }

    public async Task<TableAndSectionViewModel?> DeleteSection(int id,string userId)
    {
        // first also delete all tables  of this section and check if any table is occupied then not delete this section
        bool isTableDeleted = await _tableRepository.DeleteTableBySectionId(id,userId);
        if(isTableDeleted == false)
            return null;
        await _sectionRepository.DeleteSection(id,userId);
        var TSPage = await GetTableSectionPage();
        return TSPage;
    }
    
    public async Task<int> GetTablesCountbySectionId(int id,string? searchString)
    {
        int tr = await _tableRepository.GetTablesCount(id,searchString);
        return tr;
    }

    public async Task<AddEditTableViewModel> GetTableById(int? id)
    {
        var table = await _tableRepository.GetTableById(id);
        var sections = await _sectionRepository.GetAllSections();
        var TableDetail = new AddEditTableViewModel{
            Id = table.Id,
            Name = table.Name,
            Capacity = table.Capacity,
            Sectionid = table.Sectionid,
            Status = table.Status,
            Isavailable = table.Isavailable,
            Sections = sections
        };
        return TableDetail;
    }

    public async Task<bool?> AddTable(AddEditTableViewModel model,string userId)
    {
        bool isExist = await _tableRepository.IsTableExist(model.Name,model.Sectionid);
        if(isExist)
            return false;
        await _tableRepository.AddTable(model,userId);
        return true;
    }

    public async Task<bool?> EditTable(AddEditTableViewModel model,string userId)
    {
        bool isExist = await _tableRepository.IsEditTableExist(model.Name,model.Id,model.Sectionid);
        if(isExist)
            return false;
        await _tableRepository.EditTable(model,userId);
        return true;
    }

    public async Task<bool> DeleteTable(int id,string userId)
    {
        return await _tableRepository.DeleteTable(id,userId);
    }

    public async Task<bool> DeleteMultipleTable(int[] ids,string userId)
    {
        return await _tableRepository.DeleteMultipleTable(ids,userId);
    }
}
