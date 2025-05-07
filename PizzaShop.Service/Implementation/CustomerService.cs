using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IWebHostEnvironment _webHost;
    public CustomerService(ICustomerRepository customerRepository,IWebHostEnvironment webHost)
    {
        _customerRepository = customerRepository;
        _webHost = webHost;
    }

    public async Task<CustomerPageViewModel> GetCustomerPage()
    {
        PaginationViewModel pagination = new PaginationViewModel{
            PageIndex = 1,
            PageSize = 5,
            SearchString = "",
            FromDate = null,
            ToDate = null,
            Time = "All Time",
            SortingBy = "Id",
            SortingType = "ascending"
        };

        CustomerPageViewModel customerPage = await GetFilteredCustomer(pagination);
        return customerPage;
    }
    
    public async Task<CustomerPageViewModel> GetFilteredCustomer(PaginationViewModel pagination)
    {
        var customerPage = new CustomerPageViewModel();
        var customers = await _customerRepository.GetCustomers(pagination);
        var tr = await _customerRepository.GetCustomersCount(pagination);
        var customerList = new List<CustomerViewModel>();
        pagination.TotalRecord = tr.Count();
        pagination.TotalPage = (int)Math.Ceiling(tr.Count() / (double)pagination.PageSize);
        customerPage.Pagination = pagination;

        if (customers.Count() > 0)
        {
            foreach (var c in customers)
            {
                customerList.Add(new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Date = DateOnly.FromDateTime((DateTime)c.Createddate).ToString(),
                    CreatedDate = c.Createddate,
                    CreatedBy = c.Createdby,
                    UpdatedBy = c.Updatedby,
                    UpdatedDate = c.Updateddate,
                    TotalOrders = c.Orders.Count()
                });
            }
        }
        customerPage.CustomerList = customerList;
        return customerPage;
    }

    public async Task<FileContentResult> ExportCustomersExcel(PaginationViewModel pagination)
    {
        var customerList = await _customerRepository.GetCustomersCount(pagination);
        var totalRecord = customerList.Count();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("OrderList");
            var fromDate = string.IsNullOrEmpty(pagination.FromDate.ToString()) ? "" : DateOnly.FromDateTime((DateTime)pagination.FromDate).ToString();
            var toDate = string.IsNullOrEmpty(pagination.FromDate.ToString()) ? "" : DateOnly.FromDateTime((DateTime)pagination.ToDate).ToString();

            worksheet.Cells[9, 1, 9, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[9, 1, 9, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));

            worksheet.Cells[2, 1, 3, 2].Merge = true;
            worksheet.Cells[2, 1, 3, 2].Value = "Account";
            worksheet.Cells[2, 1, 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 1, 3, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 1, 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 1, 3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
            worksheet.Cells[2, 1, 3, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);

            worksheet.Cells[2, 3, 3, 6].Merge = true;
            worksheet.Cells[2, 3, 3, 6].Value = "PIZZASHOP";
            worksheet.Cells[2, 3, 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 3, 3, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 3, 3, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 3, 3, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            worksheet.Cells[2, 3, 3, 6].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            worksheet.Cells[2, 3, 3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[2, 8, 3, 9].Merge = true;
            worksheet.Cells[2, 8, 3, 9].Value = "Search Text:";
            worksheet.Cells[2, 8, 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 8, 3, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 8, 3, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 8, 3, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
            worksheet.Cells[2, 8, 3, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

            worksheet.Cells[2, 10, 3, 13].Merge = true;
            worksheet.Cells[2, 10, 3, 13].Value = pagination.SearchString;
            worksheet.Cells[2, 10, 3, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 10, 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 10, 3, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 10, 3, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            worksheet.Cells[2, 10, 3, 13].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            worksheet.Cells[2, 10, 3, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[5, 1, 6, 2].Merge = true;
            worksheet.Cells[5, 1, 6, 2].Value = "Date:";
            worksheet.Cells[5, 1, 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[5, 1, 6, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[5, 1, 6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[5, 1, 6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
            worksheet.Cells[5, 1, 6, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);

            worksheet.Cells[5, 3, 6, 6].Merge = true;
            worksheet.Cells[5, 3, 6, 6].Value = pagination.Time == "Select Date Range" ? "From: " + fromDate + " To: " + toDate : pagination.Time;
            worksheet.Cells[5, 3, 6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[5, 3, 6, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[5, 3, 6, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[5, 3, 6, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            worksheet.Cells[5, 3, 6, 6].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            worksheet.Cells[5, 3, 6, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells[5, 8, 6, 9].Merge = true;
            worksheet.Cells[5, 8, 6, 9].Value = "No of Records:";
            worksheet.Cells[5, 8, 6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[5, 8, 6, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[5, 8, 6, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[5, 8, 6, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
            worksheet.Cells[5, 8, 6, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

            worksheet.Cells[5, 10, 6, 13].Merge = true;
            worksheet.Cells[5, 10, 6, 13].Value = totalRecord;
            worksheet.Cells[5, 10, 6, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[5, 10, 6, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[5, 10, 6, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[5, 10, 6, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            worksheet.Cells[5, 10, 6, 13].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            worksheet.Cells[5, 10, 6, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            string path = _webHost.WebRootPath;
            string imagePath = Path.Combine(path, "images", "logos", "pizzashop_logo.png");
            if (System.IO.File.Exists(imagePath))
            {
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    var ExcelImage = worksheet.Drawings.AddPicture("logo", stream);
                    ExcelImage.SetPosition(1, 0, 14, 0);
                    ExcelImage.SetSize(150, 100);
                }
            }



            // worksheet.Cells[9, 1, 9, 1].Merge = true;
            worksheet.Cells[9, 1].Value = "Id";
            worksheet.Cells[9, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[9, 2, 9, 4].Merge = true;
            worksheet.Cells[9, 2, 9, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 2, 9, 4].Value = "Name";

            worksheet.Cells[9, 5, 9, 8].Merge = true;
            worksheet.Cells[9, 5, 9, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 5, 9, 8].Value = "Email";

            worksheet.Cells[9, 9, 9, 11].Merge = true;
            worksheet.Cells[9, 9, 9, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 9, 9, 11].Value = "Date";

            worksheet.Cells[9, 12, 9, 14].Merge = true;
            worksheet.Cells[9, 12, 9, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 12, 9, 14].Value = "Mobile Number";

            worksheet.Cells[9, 15, 9, 16].Merge = true;
            worksheet.Cells[9, 15, 9, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 15, 9, 16].Value = "Total Order";


            int row = 10;
            if (customerList.Count > 0)
            {
                foreach (var c in customerList)
                {
                    worksheet.Cells[row, 1, row, 1].Value = c.Id;
                    worksheet.Cells[row, 1, row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[row, 2, row, 4].Merge = true;
                    worksheet.Cells[row, 2, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2, row, 4].Value = c.Name;

                    worksheet.Cells[row, 5, row, 8].Merge = true;
                    worksheet.Cells[row, 5, row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5, row, 8].Value = c.Email;

                    worksheet.Cells[row, 9, row, 11].Merge = true;
                    worksheet.Cells[row, 9, row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 9, row, 11].Value = DateOnly.FromDateTime((DateTime)c.Date).ToString();

                    worksheet.Cells[row, 12, row, 14].Merge = true;
                    worksheet.Cells[row, 12, row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 12, row, 14].Value = c.Phone;


                    worksheet.Cells[row, 15, row, 16].Merge = true;
                    worksheet.Cells[row, 15, row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 15, row, 16].Value = c.Orders.Count();
                    row++;
                }
            }
            else
            {
                worksheet.Cells[10, 1, 10, 16].Merge = true;
                worksheet.Cells[10, 1, 10, 16].Value = "No Record Found";
                worksheet.Cells[10, 1, 10, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            }

            // Auto-fit columns for better readability
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            // Convert the package to a byte array
            var fileBytes = package.GetAsByteArray();
            return new FileContentResult(
                fileBytes, //Excel File data in Byte Array
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" //Excel Sheet Mime Type
            )
            {
                FileDownloadName = "Customer.xlsx" //File Name
            };
        }
    }

    public async Task<CustomerHistoryViewModel> GetCustomerHistory(int id)
    {
        var customer = await _customerRepository.GetCustomerHistory(id);
        List<CustomerOrderViewModel> orderList = customer.Orders?.Select(o => new CustomerOrderViewModel{
                    Id = o.Id,
                    OrderType = "DineIn",
                    PaymentStatus = o.Payment?.Status,
                    Items = (int)(o.Orderitems?.Count()),
                    Amount = o.Totalamount,
                    OrderDate = DateOnly.FromDateTime((DateTime)o.Createddate).ToString()
        }).ToList();

        var customerHistory = new CustomerHistoryViewModel{
            Id = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            ComingSince = DateOnly.FromDateTime((DateTime)customer.Date).ToString(),
            Visits = customer.Orders?.Count(),
            MaxOrder = customer.Orders.Count() > 0 ? customer.Orders.Select(o => o.Totalamount).Max() : 0,
            AverageBill = customer.Orders.Count() > 0 ? customer.Orders.Select(o => o.Totalamount).Average() : 0,
            OrderList = orderList
        };

        return customerHistory;
        
    }
}
