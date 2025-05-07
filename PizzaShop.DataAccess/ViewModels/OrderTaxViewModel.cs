namespace PizzaShop.DataAccess.ViewModels;

public class OrderTaxViewModel
{
    public int Id { get; set; }

    public int? Orderid { get; set; }

    public string? Name { get; set; }

    public decimal? Taxvalue { get; set; }

    public decimal? TaxAmount { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsActive { get; set; }

    public string? Type { get; set; }
}
