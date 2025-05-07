using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class WaitingTokenRepository : IWaitingTokenRepository
{
  private readonly PizzashopContext _context;

  public WaitingTokenRepository(PizzashopContext context)
  {
    _context = context;
  }

  public async Task<List<Waitingtoken>> GetWaitingTokensBySectionId(int? id)
  {
    return await _context.Waitingtokens.Where(w => w.Isdeleted == false && w.Isassigned == false && (w.Sectionid == id ||id == 0))
                .Include(w => w.Section)
                .Include(w => w.Customer)
                .ToListAsync();
  }

  public async Task<Waitingtoken> GetWaitingTokeById(int? id)
  {
    Waitingtoken Wt = await _context.Waitingtokens.Where(w => w.Id == id && w.Isdeleted == false)
                      .Include(w => w.Customer)
                      .FirstOrDefaultAsync();
    return Wt;
  }

  public async Task AddWaitingToken(AddEditWaitingTokenViewModel model, string userId)
  {
    Waitingtoken newWtToken = new Waitingtoken
    {
      Customerid = model.CustomerId,
      Isassigned = false,
      Noofpersons = model.NoOfPersons,
      Sectionid = model.SectionId,
      Createddate = DateTime.Now,
      Createdby = userId,
      Isdeleted = false
    };

    await _context.Waitingtokens.AddAsync(newWtToken);
    await _context.SaveChangesAsync();
  }


  public async Task EditWaitingToken(AddEditWaitingTokenViewModel model, string userId)
  {
    Waitingtoken Wt = await _context.Waitingtokens.FirstOrDefaultAsync(w => w.Id == model.Id);

    Wt.Noofpersons = model.NoOfPersons;
    Wt.Sectionid = model.SectionId;
    Wt.Isassigned = model.IsAssigned == null ? false : model.IsAssigned;
    Wt.Updateddate = DateTime.Now;
    Wt.Updatedby = userId;

    await _context.SaveChangesAsync();
  }

  public async Task DeleteWaitingToken(int id, string userId)
  {
    Waitingtoken Wt = await _context.Waitingtokens.FirstOrDefaultAsync(w => w.Id == id);
    Wt.Isdeleted = true;
    Wt.Updatedby = userId;
    Wt.Updateddate = DateTime.Now;
    await _context.SaveChangesAsync();
  }

}
