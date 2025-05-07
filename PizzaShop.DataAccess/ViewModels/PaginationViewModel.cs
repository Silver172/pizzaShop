namespace PizzaShop.DataAccess.ViewModels;

public class PaginationViewModel
{
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public string? SearchString { get; set; }
    public int TotalPage { get; set; }
    public int TotalRecord { get; set; }
    public DateTime? FromDate {get;set;}
    public DateTime? ToDate {get;set;}
    public string? Status {get;set;}
    public string? Time {get;set;}
    public string? SortingType {get;set;}
    public string? SortingBy {get;set;}
    public string? FilterBy {get;set;}
}
