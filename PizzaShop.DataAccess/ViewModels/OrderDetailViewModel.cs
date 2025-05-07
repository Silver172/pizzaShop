using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.ViewModels;

public class OrderDetailViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public string InvoiceNo { get; set; } = null!;
    public string? Phone { get; set; } = null!;
    public short NoOfPerson { get; set; }
    public string Email { get; set; } = null!;
    public List<string> Table { get; set; } = null!;
    public string Section { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? PaidOn { get; set; }
    public string? PlacedOn { get; set; }
    public string? ModifierOn { get; set; }
    public string? OrderDuration { get; set; }
    public string PaymentMode { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal? Discount { get; set; }
    public bool? IsSGSTSeclected { get; set; }
    public string? Comments { get; set; }
    public List<OrderItemViewModel> OrderItems {get; set;} = null! ;
    public List<OrderTaxViewModel>? OrderTax { get; set; }
}
