namespace PizzaShop.DataAccess.ViewModels;

public class WaitingListViewModel
{
  public int Id { get; set; }

  public string CustomerName { get; set; }

  public string Email { get; set; }

  public string? Phone { get; set; }

  public bool? IsAssigned { get; set; }

  public int? SectionId { get; set; }

  public short NoOfPersons { get; set; }

  public string? WaitingTime { get; set; }

  public string? CreatedDate { get; set; }

  public string? CreatedBy { get; set; }

  public DateTime? UpdatedDate { get; set; }

  public string? UpdatedBy { get; set; }

}
