using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class SectionRepository : ISectionRepository
{
    private readonly PizzashopContext _context;

    public SectionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<SectionViewModel>> GetAllSections()
    {
        var sections = await _context.Sections.Where(s => s.Isdeleted == false).OrderBy(s => s.Id).ToListAsync();
        var sectionList = new List<SectionViewModel>();

        if (sections.Count > 0)
        {
            foreach (var s in sections)
            {
                sectionList.Add(new SectionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                });
            }
        }

        return sectionList;
    }

    public async Task<bool> IsSectionExist(string name)
    {
        int count = await _context.Sections.Where(s => s.Name.ToLower() == name.ToLower() && s.Isdeleted == false).CountAsync();
        if (count > 0)
            return true;
        return false;
    }

    public async Task<bool> isEditSectionExist(string name, int? id)
    {
        var section = await _context.Sections.Where(s => s.Isdeleted == false && s.Name.ToLower() == name.ToLower()).ToListAsync();
        foreach (var s in section)
        {
            if (s.Id != id)
                return true;
        }
        return false;
    }

    public async Task AddSection(AddEditSectionViewModel model, string userId)
    {
        var newSection = new Section
        {
            Name = model.Name,
            Description = model.Description,
            Isdeleted = false,
            Createdby = userId,
            Createddate = DateTime.Now
        };

        await _context.Sections.AddAsync(newSection);
        await _context.SaveChangesAsync();
    }

    public async Task<Section?> GetSectionById(int? id)
    {
        return await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task EditSection(AddEditSectionViewModel model, string userId)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == model.Id);
        if (section != null)
        {
            section.Name = model.Name;
            section.Description = model.Description;
            section.Updateddate = DateTime.Now;
            section.Updatedby = userId;
            _context.SaveChanges();
        }
    }

    public async Task DeleteSection(int id, string userId)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
        if (section != null)
        {
            section.Isdeleted = true;
            section.Updatedby = userId;
            section.Updateddate = DateTime.Now;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<Section>> GetTableViewOrderApp()
    {
        var section = await _context.Sections.Where(s => s.Isdeleted == false).Include(s => s.Tables)
                                            .ThenInclude(t => t.Tableordermappings)
                                            .ThenInclude(t => t.Order)
                                            .ToListAsync();
        return section;
    }

    public async Task<List<Section>> GetSectionsForWaitingList()
    {
        return await _context.Sections.Where(s => s.Isdeleted == false)
            .Include(s => s.Waitingtokens)
            .ToListAsync();
    }

    public async Task<List<Section>> getSections()
    {
        var sections = await _context.Sections.Where(s => s.Isdeleted == false && s.Tables.Any(t => t.Isavailable == true)).ToListAsync();
        return sections;
    }
}
