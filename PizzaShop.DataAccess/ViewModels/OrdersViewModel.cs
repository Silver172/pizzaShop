namespace PizzaShop.DataAccess.ViewModels;

public class OrdersViewModel
{
    public int Id { get; set; }

    public string CustomerName { get; set; }

    public decimal Totalamount { get; set; }

    public string Status { get; set; } = null!;
    public string? PaymentMode { get; set; }
    public Decimal? Rating { get; set;   }

    public bool? Isdeleted { get; set; }

    public DateOnly Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }


}
