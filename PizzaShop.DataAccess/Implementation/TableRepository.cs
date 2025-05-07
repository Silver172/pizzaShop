using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class TableRepository : ITableRepository
{
    private readonly PizzashopContext _context;

    public TableRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<TableViewModel>> GetAllTables()
    {
        var tables = await _context.Tables.Where(t => t.Isdeleted == false).ToListAsync();
        var tableList = new List<TableViewModel>();

        if (tables.Count > 0)
        {
            foreach (var t in tables)
            {
                tableList.Add(new TableViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Sectionid = t.Sectionid,
                    Capacity = t.Capacity,
                    Status = t.Status,
                    Isavailable = t.Isavailable
                });
            }
        }
        return tableList;
    }

    public async Task<List<TableViewModel>> GetTablesBySectionId(int id, PaginationViewModel pagination)
    {
        var query = _context.Tables.Where(m => m.Isdeleted == false && m.Sectionid == id);

        if (!string.IsNullOrEmpty(pagination.SearchString))
        {
            var searchString = pagination.SearchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchString));
        }

        var tables = await query.OrderBy(i => i.Name)
            .Skip((pagination.PageIndex - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var tableList = new List<TableViewModel>();

        if (tables.Count > 0)
        {
            foreach (var t in tables)
            {
                tableList.Add(new TableViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Sectionid = t.Sectionid,
                    Capacity = t.Capacity,
                    Status = t.Status,
                    Isavailable = t.Isavailable
                });
            }
        }
        return tableList;
    }

    public async Task<int> GetTablesCount(int id,string? searchString)
    {
        var query = _context.Tables.Where(m => m.Isdeleted == false && m.Sectionid == id);

        if (!string.IsNullOrEmpty(searchString))
        {
            var ss = searchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(ss));
        }

        var count = await query.OrderBy(i => i.Name)
            .ToListAsync();
        return count.Count();
    }

    public async Task<bool> IsTableExist(string name, int? sectionId)
    {
        var table = await _context.Tables.Where(t => t.Isdeleted == false && t.Name.ToLower() == name.ToLower() && t.Sectionid == sectionId).ToListAsync();
        if (table.Count() > 0)
            return true;
        return false;
    }

    public async Task<bool> IsEditTableExist(string name, int? id, int? sectionId)
    {
        var modifier = await _context.Tables.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower() && m.Sectionid == sectionId).ToListAsync();
        foreach (var m in modifier)
        {
            if (m.Id != id)
                return true;
        }
        return false;
    }

    public async Task<bool> DeleteTableBySectionId(int id,string userId)
    {
        var tables = await _context.Tables.Where(t => t.Isdeleted == false && t.Sectionid == id).ToListAsync();

        foreach (var t in tables)
        {
            if (t.Status == "Occupied")
                return false;
            else
            {
                t.Isdeleted = true;
                t.Updateddate = DateTime.Now;
                t.Updatedby = userId;
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Table?> GetTableById(int? id)
    {
        return await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddTable(AddEditTableViewModel model, string userId)
    {
        var newTable = new Table()
        {
            Name = model.Name,
            Capacity = model.Capacity,
            Sectionid = model.Sectionid,
            Status = model.Status,
            Isavailable = model.Status == "Occupied" ? false : true,
            Createdby = userId,
            Createddate = DateTime.Now,
        };
        await _context.Tables.AddAsync(newTable);
        await _context.SaveChangesAsync();
    }

    public async Task EditTable(AddEditTableViewModel model, string userId)
    {
        var editTable = await _context.Tables.FirstOrDefaultAsync(t => t.Id == model.Id);
        if (editTable != null)
        {
            editTable.Name = model.Name;
            editTable.Capacity = model.Capacity;
            editTable.Sectionid = model.Sectionid;
            editTable.Status = model.Status;
            editTable.Isavailable = model.Status == "Occupied" ? false : true;
            editTable.Updatedby = userId;
            editTable.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> DeleteTable(int id,string userId)
    {
        var deleteTable = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
        if(deleteTable.Isavailable == false)
            return false;
        deleteTable.Isdeleted = true;
        deleteTable.Updateddate = DateTime.Now;
        deleteTable.Updatedby = userId;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteMultipleTable(int[] ids,string userId)
    {
        foreach(var id in ids)
        {
            var deleteTable = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if(deleteTable.Isavailable == false)
                return false;
            deleteTable.Isdeleted = true;
            deleteTable.Updatedby = userId;
            deleteTable.Updateddate = DateTime.Now;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Table>> GetTablesBySectionIdForWaitingtoken(int id)
    {
        var tables = await _context.Tables.Where(t => t.Isdeleted == false && t.Sectionid == id && t.Isavailable == true).ToListAsync();
        return tables;
    }

    public async Task<List<Table>> GetOptimizedTablesBySectionId(int? id,int noOfPersons,List<int> tableIds)
    {
        var tables = await _context.Tables.Where(t => t.Isdeleted == false && t.Sectionid == id && t.Isavailable == true && t.Capacity >= noOfPersons && tableIds.Contains(t.Id) == false).ToListAsync();
        return tables;
    }

    public async Task UpdateTableStatus(int id, string status, string userId)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id && t.Isdeleted == false);
        if (table != null)
        {
            table.Status = status;
            table.Isavailable = status == "Occupied" ? false : true;
            table.Updatedby = userId;
            table.Updateddate = DateTime.Now;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Table>> GetAvailableTablesBySectionId(int id)
    {
        return await _context.Tables.Where(t => t.Isdeleted == false && t.Sectionid == id && t.Isavailable == true).ToListAsync();
    }
}
