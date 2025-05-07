namespace PizzaShop.DataAccess.ViewModels;

public class MenuOrderDetailsViewModal
{
  public int Id { get; set; }
  public List<string> Table { get; set; } = null!;
  public string Section { get; set; } = null!;
  public string? PaymentMethod { get; set; }
  public bool? IsSGSTSeclected { get; set; }
  public decimal TotalAmount { get; set; }
  public decimal SubtotalAmount { get; set; }
  public decimal? Discount { get; set; }
  public List<OrderItemViewModel> OrderItems = null!;
  public List<OrderTaxViewModel>? OrderTax { get; set; }
}
