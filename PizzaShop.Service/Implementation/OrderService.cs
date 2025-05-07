using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using System.Globalization;
using System.Threading.Tasks;
namespace PizzaShop.Service.Implementation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepositroy;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWebHostEnvironment _webHost;
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ITaxesAndFeesRepository _taxAndFeesRepository;
    public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepositroy, IFeedbackRepository feedbackRepository, IPaymentRepository paymentRepository, IWebHostEnvironment webHost, ITableRepository tableRepository, ISectionRepository sectionRepository, ITaxesAndFeesRepository taxAndFeesRepository)
    {
        _orderRepository = orderRepository;
        _customerRepositroy = customerRepositroy;
        _feedbackRepository = feedbackRepository;
        _paymentRepository = paymentRepository;
        _webHost = webHost;
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _taxAndFeesRepository = taxAndFeesRepository;
    }

    public async Task<OrdersPageViewModel> GetOrders(PaginationViewModel pagination)
    {
        var orders = await _orderRepository.GetOrderList(pagination);
        var orderList = new List<OrdersViewModel>();
        var orderPage = new OrdersPageViewModel();
        orderPage.Pagination = pagination;
        orderPage.OrdersList = orderList;
        if (orders.Count > 0)
        {

            foreach (var o in orders)
            {
                string cname = o.Customerid != null ? (await _customerRepositroy.GetCustomerById(o.Customerid)).Name : "";
                string pMethod = o.Paymentid != null ? (await _paymentRepository.GetPaymentById(o.Paymentid)).Method : "";
                var feedback = await _feedbackRepository.GetFeedbackByOrderId(o.Id);
                decimal? avgRating = feedback != null ? feedback?.Avgrating : 0;
                DateOnly dateOnly = DateOnly.FromDateTime((DateTime)o.Createddate);
                orderList.Add(new OrdersViewModel
                {
                    Id = o.Id,
                    CustomerName = cname,
                    Totalamount = o.Totalamount,
                    Status = o.Status,
                    Createddate = dateOnly,
                    PaymentMode = pMethod,
                    Rating = avgRating == null ? 0 : avgRating
                });
            }

            var tr = (await _orderRepository.GetOrdersCount(pagination)).Count();
            pagination.TotalRecord = tr;
            pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);

            orderPage.OrdersList = orderList;
            return orderPage;
        }
        return orderPage;
    }

    public async Task<OrdersPageViewModel> GetOrdersWithoutPagination(PaginationViewModel pagination)
    {
        var orders = await _orderRepository.GetOrdersCount(pagination);
        var orderList = new List<OrdersViewModel>();
        var orderPage = new OrdersPageViewModel();
        orderPage.Pagination = pagination;
        orderPage.OrdersList = orderList;
        if (orders.Count > 0)
        {

            foreach (var o in orders)
            {
                string cname = o.Customerid != null ? (await _customerRepositroy.GetCustomerById(o.Customerid)).Name : "";
                string pMethod = o.Paymentid != null ? (await _paymentRepository.GetPaymentById(o.Paymentid)).Method : "";
                var feedback = await _feedbackRepository.GetFeedbackByOrderId(o.Id);
                decimal? avgRating = feedback != null ? feedback?.Avgrating : 0;
                DateOnly dateOnly = DateOnly.FromDateTime((DateTime)o.Createddate);
                orderList.Add(new OrdersViewModel
                {
                    Id = o.Id,
                    CustomerName = cname,
                    Totalamount = o.Totalamount,
                    Status = o.Status,
                    Createddate = dateOnly,
                    PaymentMode = pMethod,
                    Rating = avgRating
                });
            }

            var tr = orders.Count();
            pagination.TotalRecord = tr;
            pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);

            orderPage.OrdersList = orderList;
            return orderPage;
        }
        return orderPage;
    }
    public async Task<FileContentResult> ExportOrderExcel(PaginationViewModel pagination)
    {
        OrdersPageViewModel OrderPage = await GetOrdersWithoutPagination(pagination);
        var orderList = OrderPage.OrdersList;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("OrderList");



            worksheet.Cells[9, 1, 9, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[9, 1, 9, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));

            worksheet.Cells[2, 1, 3, 2].Merge = true;
            worksheet.Cells[2, 1, 3, 2].Value = "Status";
            worksheet.Cells[2, 1, 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 1, 3, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 1, 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 1, 3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
            worksheet.Cells[2, 1, 3, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);

            worksheet.Cells[2, 3, 3, 6].Merge = true;
            worksheet.Cells[2, 3, 3, 6].Value = pagination.Status;
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
            worksheet.Cells[5, 3, 6, 6].Value = pagination.Time;
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
            worksheet.Cells[5, 10, 6, 13].Value = OrderPage.Pagination.TotalRecord;
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
            worksheet.Cells[9, 2, 9, 4].Value = "Date";

            worksheet.Cells[9, 5, 9, 7].Merge = true;
            worksheet.Cells[9, 5, 9, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 5, 9, 7].Value = "Customer";

            worksheet.Cells[9, 8, 9, 10].Merge = true;
            worksheet.Cells[9, 8, 9, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 8, 9, 10].Value = "Status";

            worksheet.Cells[9, 11, 9, 12].Merge = true;
            worksheet.Cells[9, 11, 9, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 11, 9, 12].Value = "Payment Mode";

            worksheet.Cells[9, 13, 9, 14].Merge = true;
            worksheet.Cells[9, 13, 9, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 13, 9, 14].Value = "Rating";

            worksheet.Cells[9, 15, 9, 16].Merge = true;
            worksheet.Cells[9, 15, 9, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[9, 15, 9, 16].Value = "Total Amount";

            int row = 10;
            if (orderList.Count > 0)
            {
                foreach (var o in orderList)
                {
                    worksheet.Cells[row, 1, row, 1].Value = o.Id;
                    worksheet.Cells[row, 1, row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[row, 2, row, 4].Merge = true;
                    worksheet.Cells[row, 2, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2, row, 4].Value = o.Createddate;

                    worksheet.Cells[row, 5, row, 7].Merge = true;
                    worksheet.Cells[row, 5, row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5, row, 7].Value = o.CustomerName;

                    worksheet.Cells[row, 8, row, 10].Merge = true;
                    worksheet.Cells[row, 8, row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 8, row, 10].Value = o.Status;

                    worksheet.Cells[row, 11, row, 12].Merge = true;
                    worksheet.Cells[row, 11, row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 11, row, 12].Value = o.PaymentMode;


                    worksheet.Cells[row, 13, row, 14].Merge = true;
                    worksheet.Cells[row, 13, row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 13, row, 14].Value = o.Rating;


                    worksheet.Cells[row, 15, row, 16].Merge = true;
                    worksheet.Cells[row, 15, row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 15, row, 16].Value = o.Totalamount;
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
                FileDownloadName = "Orders.xlsx" //File Name
            };
        }
    }

    public async Task<OrderDetailViewModel> OrderDetails(int id)
    {
        var order = await _orderRepository.OrderDetails(id);

        if (order == null)
        {
            throw new Exception("Order not found");
        }

        decimal orderTotalAmount = 0;
        decimal orderTaxTotalAmount = 0;
        var taxes = order.Ordertaxmappings?.ToList();
        List<OrderTaxViewModel> OrderTaxesList = new List<OrderTaxViewModel>();
        if (taxes != null)
        {
            foreach (var i in order.Ordertaxmappings)
            {
                OrderTaxesList.Add(new OrderTaxViewModel
                {
                    Id = i.Tax.Id,
                    Taxvalue = i.Taxvalue,
                    Name = i.Tax.Name
                });
                decimal tt = (decimal)(i.Tax.Flatamount == null ? 0 : i.Tax.Flatamount);
                orderTaxTotalAmount = orderTaxTotalAmount +  tt;
            }
        }
        List<OrderItemViewModel> orderItems = order.Orderitems.Select(item => new OrderItemViewModel
        {
            Id = item.Id,
            Menuitemid = item.Menuitemid,
            Orderid = item.Orderid,
            Comment = item.Comment,
            Name = item.Name,
            Quantity = item.Quantity,
            Rate = item.Rate,
            Amount = item.Amount,
            Tax = item.Tax,
            TotalAmount = item.Quantity * item.Rate,
            Modifiers = (List<OrderItemModifierViewModel>)item.Orderitemmodifiermappings.Select(modifier => new OrderItemModifierViewModel
            {
                Id = modifier.Id,
                OrderItemid = modifier.Orderitemid,
                Name = modifier.Modifier?.Name,
                Quantity = modifier.Quantityofmodifier,
                Rate = modifier.Rateofmodifier,
                TotalAmount = modifier.Rateofmodifier * modifier.Quantityofmodifier
            }).ToList()
        }).ToList();

        foreach (var item in orderItems)
        {
            orderTotalAmount += (decimal)item.TotalAmount;
            foreach (var m in item.Modifiers)
            {   
                orderTotalAmount = orderTotalAmount + (decimal)m.TotalAmount;
            }            
        }

        DateOnly dateOnly = DateOnly.FromDateTime((DateTime)order.Createddate);
        string invoiceNumber = "#DOM" + Convert.ToDateTime(order.Createddate).ToString("yyyyMMdd") + "-" + order.Id.ToString();
        var model = new OrderDetailViewModel 
        {
            Id = order.Id,
            CustomerName = order.Customer.Name,
            InvoiceNo = invoiceNumber,
            TotalAmount = order.Totalamount,
            SubtotalAmount = order.Subtotalamount,
            Discount = order.Discount,
            PlacedOn = order.Createddate.ToString(),
            PaidOn = order.Status == "Completed" ? order.Updateddate.ToString() : "",
            Status = order.Status,
            Email = order.Customer?.Email,
            PaymentMode = order.Status == "Completed" ? "Cash" : "Pending",
            Phone = order.Customer?.Phone,
            NoOfPerson = (short)order.Tableordermappings?.Select(s => s.Noofpersons).FirstOrDefault(),
            Table = order.Tableordermappings.Select(t => t.Table.Name).ToList(),
            Section = order.Tableordermappings.Select(s => s.Table.Section.Name).FirstOrDefault(),
            OrderItems = orderItems,
            OrderTax = OrderTaxesList
        };
        return model;
    }

}
