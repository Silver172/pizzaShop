using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IWaitingTokenRepository
{
  public Task<List<Waitingtoken>> GetWaitingTokensBySectionId(int? id);
  public Task<Waitingtoken> GetWaitingTokeById(int? id);
  public Task AddWaitingToken(AddEditWaitingTokenViewModel model, string userId);
  public Task EditWaitingToken(AddEditWaitingTokenViewModel model, string userId);
  public Task DeleteWaitingToken(int id, string userId);
}
