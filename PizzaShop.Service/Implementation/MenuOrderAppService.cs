using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Implementation;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class MenuOrderAppService : IMenuOrderAppService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository;
    private readonly IMappingMenuItemWithModifierRepository _mappingMenuItemWithModifierRepository;
    private readonly ITaxesAndFeesRepository _taxesAndFeesRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderItemModifierMappingRepository _orderItemModifierMappingRepository;
    private readonly IOrderTaxMappingRepository _orderTaxMappingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITableOrderMappingRepository _tableOrderMappingRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    public MenuOrderAppService(
        IMenuItemRepository menuItemRepository,
        ICategoryRepository categoryRepository,
        IOrderRepository orderRepository,
        IModifierGroupRepository modifierGroupRepository,
        IMappingMenuItemWithModifierRepository mappingMenuItemWithModifierRepository,
        ITaxesAndFeesRepository taxesAndFeesRepository,
        IOrderItemRepository orderItemRepository,
        IOrderItemModifierMappingRepository orderItemModifierMappingRepository,
        IOrderTaxMappingRepository orderTaxMappingRepository,
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        ITableOrderMappingRepository tableOrderMappingRepository,
        ITableRepository tableRepository,
        IFeedbackRepository feedbackRepository,
        IInvoiceRepository invoiceRepository
    )
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _mappingMenuItemWithModifierRepository = mappingMenuItemWithModifierRepository;
        _taxesAndFeesRepository = taxesAndFeesRepository;
        _orderItemRepository = orderItemRepository;
        _orderItemModifierMappingRepository = orderItemModifierMappingRepository;
        _orderTaxMappingRepository = orderTaxMappingRepository;
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _tableOrderMappingRepository = tableOrderMappingRepository;
        _tableRepository = tableRepository;
        _feedbackRepository = feedbackRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<MenuOrderAppViewModel> GetMenuItemsByCategoryId(int cId, string? searchString)
    {
        MenuOrderAppViewModel menuOrderAppPage = new MenuOrderAppViewModel();
        menuOrderAppPage.OrderDetails = null;
        List<Category> Categories = await _categoryRepository.GetAllCategories();

        var categoryList = new List<CategoriesViewModel>();

        foreach (var cat in Categories)
        {
            categoryList.Add(new CategoriesViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            });
        }

        var MenuItem = await _menuItemRepository.GetMenuItemsByCategoryId(cId, searchString);
        var itemList = new List<ItemsViewModel>();
        foreach (var item in MenuItem)
        {
            itemList.Add(new ItemsViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Itemtype = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Isavailable = item.Isavailable,
                Itemimage = item.Itemimage,
                Isfavourite = item.Isfavourite,
            });
        }

        menuOrderAppPage.ItemsList = itemList;
        menuOrderAppPage.CategoriesList = categoryList;
        return menuOrderAppPage;
    }


    public async Task<List<ItemModifierViewModel>> GetModifiersByItemId(int itemId)
    {
        List<ItemModifierViewModel> itemModifiers = new List<ItemModifierViewModel>();
        List<Mappingmenuitemwithmodifier> mappings = await _mappingMenuItemWithModifierRepository.GetMappingsByItemId(itemId);

        foreach (var m in mappings)
        {
            itemModifiers.Add(new ItemModifierViewModel
            {
                Id = m.Id,
                ModifierGroupId = (int)m.Modifiergroupid,
                Name = m.Modifiergroup.Name,
                Minselectionrequired = m.Minselectionrequired,
                Maxselectionrequired = m.Maxselectionrequired,
                ModifierList = m.Modifiergroup.Modifiers.Where(m => m.Isdeleted == false).Select(mod => new ModifierViewModel
                {
                    Id = mod.Id,
                    Name = mod.Name,
                    Rate = mod.Rate,
                    Quantity = mod.Quantity,
                    Description = mod.Description,
                    Modifiergroupid = mod.Modifiergroupid,
                    Unitid = mod.Unitid,
                }).ToList()
            });
        }

        return itemModifiers;
    }

    public async Task<MenuOrderAppViewModel> GetMenuWithOrderDetail(int? orderId)
    {
        var order = await _orderRepository.OrderDetails((int)orderId);
        MenuOrderAppViewModel menuOrderAppPage = await GetMenuItemsByCategoryId(0, "");
        menuOrderAppPage.OrderDetails = new OrderDetailViewModel();

        if (order == null)
        {
            return menuOrderAppPage;
        }

        decimal orderTotalAmount = 0;
        decimal orderTaxTotalAmount = 0;
        var taxes = order.Ordertaxmappings?.ToList();

        List<OrderItemViewModel> orderItems = order.Orderitems.Select(item => new OrderItemViewModel
        {
            Id = item.Id,
            Menuitemid = item.Menuitemid,
            Orderid = item.Orderid,
            Comment = item.Comment,
            Name = item.Name,
            Quantity = item.Quantity,
            Rate = item.Rate,
            Amount = item.Quantity * item.Rate,
            Tax = item.Tax,
            TotalAmount = (item.Quantity * item.Rate) + item.Totalmodifieramount,
            TotalModifierAmount = item.Totalmodifieramount,
            Instruction = item.Instruction,
            Modifiers = (List<OrderItemModifierViewModel>)item.Orderitemmodifiermappings.Select(modifier => new OrderItemModifierViewModel
            {
                Id = modifier.Id,
                OrderItemid = modifier.Orderitemid,
                Name = modifier.Modifier?.Name,
                Quantity = modifier.Quantityofmodifier,
                Rate = modifier.Rateofmodifier,
                TotalAmount = modifier.Totalamount,
                ModifierId = modifier.Modifierid,
                ModifierGroupId = modifier.Modifier?.Modifiergroupid
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

        List<Taxesandfee> taxList = await _taxesAndFeesRepository.GetAllTaxesAndFees();
        
        List<OrderTaxViewModel> OrderTaxesList = order.Ordertaxmappings.Select(tax => new OrderTaxViewModel
        {
            Id = tax.Id,
            Taxvalue = tax.Taxvalue,
            Name = tax.Tax.Name,
            TaxAmount = tax.Tax.Flatamount == null ? tax.Tax.Percentge : tax.Tax.Flatamount,
            Type = tax.Tax.Flatamount == null ? "Percentage" : "Flat Amount",
            IsActive = taxList.Find(t => t.Id == tax.Tax.Id)?.Isactive,
            IsDefault = taxList.Find(t => t.Id == tax.Tax.Id)?.Isdefault
        }).ToList();

        if (OrderTaxesList.Count() == 0)
        {
            foreach (var t in taxList)
            {
                OrderTaxesList.Add(new OrderTaxViewModel
                {
                    Id = t.Id,
                    Taxvalue = 0,
                    Name = t.Name,
                    TaxAmount = t.Flatamount == null ? t.Percentge : t.Flatamount,
                    Type = t.Flatamount == null ? "Percentage" : "Flat Amount",
                    IsActive = t.Isactive,
                    IsDefault = t.Isdefault
                });
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
            Comments = order.Notes,
            IsSGSTSeclected = order.Isgstselected,
            PaymentMode = "Cash",
            Phone = order.Customer?.Phone,
            NoOfPerson = (short)order.Tableordermappings?.Select(s => s.Noofpersons).FirstOrDefault(),
            Table = order.Tableordermappings.Select(t => t.Table.Name).ToList(),
            Section = order.Tableordermappings.Select(s => s.Table.Section.Name).FirstOrDefault(),
            OrderItems = orderItems,
            OrderTax = OrderTaxesList
        };

        menuOrderAppPage.OrderDetails = model;

        return menuOrderAppPage;
    }

    public async Task<bool> MarkAsFavourite(int itemId)
    {
        return await _menuItemRepository.MarkAsFavourite(itemId);
    }

    public async Task<bool> SaveOrder(OrderDetailViewModel order, string userId)
    {

        await _orderRepository.UpdateOrder(order, userId);

        List<Orderitem> oldOrderItems = await _orderItemRepository.GetOrderItemsByOrderId(order.Id);
        List<int> deleteItemId = new List<int>();
        List<int> newItemIds = new List<int>();

        foreach (var i in order.OrderItems)
        {
            if (i.Id > 0)
                newItemIds.Add(i.Id);
        }
        foreach (var item in order.OrderItems)
        {
            if (item.Id <= 0 || item.Id == 0 )
            {
                Orderitem orderItem = new Orderitem
                {
                    Menuitemid = item.Menuitemid,
                    Orderid = order.Id,
                    Status = "In Progress",
                    Comment = item.Comment,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Rate = (decimal)item.Rate,
                    Amount = (decimal)item.Rate * item.Quantity,
                    Totalamount = (decimal)(item.TotalAmount + item.TotalModifierAmount),
                    Tax = item.Tax,
                    Totalmodifieramount = item.TotalModifierAmount,
                    Instruction = item.Instruction,
                    Readyitemquantity = 0,
                    Isdeleted = false,
                    Createddate = DateTime.Now,
                    Createdby = userId,
                };

                // add
                await _orderItemRepository.AddNewOrderItem(orderItem);
                foreach (var modifier in item.Modifiers)
                {
                    Orderitemmodifiermapping orderItemModifierMapping = new Orderitemmodifiermapping
                    {
                        Orderitemid = orderItem.Id,
                        Modifierid = modifier.ModifierId,
                        Quantityofmodifier = 1,
                        Rateofmodifier = modifier.Rate,
                        Totalamount = modifier.TotalAmount,
                        Isdeleted = false,
                        Createddate = DateTime.Now,
                        Createdby = userId
                    };
                    await _orderItemModifierMappingRepository.AddMapping(orderItemModifierMapping);
                }
            }
            else
            {
                // edit
                await _orderItemRepository.UpdateOrderItem(item, userId);
            }
        }

        foreach (var item in oldOrderItems)
        {
            if (!newItemIds.Contains(item.Id))
            {
                deleteItemId.Add(item.Id);
                // delete
                await _orderItemRepository.DeleteOrderItem(item.Id, userId);
            }
        }

        foreach (var t in order.OrderTax)
        {
            if (order.Status == "In Progress")
            {
                await _orderTaxMappingRepository.UpdateOrderTaxMapping(t, userId);
            }
            else
            {
                Ordertaxmapping newMapping = new Ordertaxmapping
                {
                    Orderid = order.Id,
                    Taxid = t.Id,
                    Taxvalue = t.Taxvalue,
                    Isdeleted = false,
                    Createdby = userId,
                    Createddate = DateTime.Now
                };
                await _orderTaxMappingRepository.AddNewOrderTaxMapping(newMapping);
            }
        }

        return true;
    }


    public async Task<bool> CheckIsItemReady(int id)
    {
        Orderitem orderitem = await _orderItemRepository.GetOrderItemById(id);
        if (orderitem != null)
        {
            if (orderitem.Readyitemquantity > 0)
                return true;
        }
        return false;
    }

    public async Task<bool> AddEditOrderComments(int id, string comment, string userId)
    {
        if (comment.Trim().Length == 0)
            return false;
        return await _orderRepository.AddEditOrderComment(id, comment, userId);
    }

    public async Task<int> UpdateCustomer(int id, string name, string email, string phone, int noOfPerson, string? userId)
    {
        List<Tableordermapping> mapping = await _tableOrderMappingRepository.GetTableOrderMappingByOrderId(id);
        int sum = 0;
        foreach (var t in mapping)
        {
            sum += t.Table.Capacity;
        }
        if (noOfPerson > sum)
            return -1;

        await _customerRepository.EditCustomer(name, email, phone, userId);
        return 1;
    }

    public async Task<bool> SaveOrderItemComment(int id, string comment, string? userId)
    {
        if (comment.Trim().Length == 0)
            return false;
        return await _orderItemRepository.SaveComment(id, comment, userId);
    }

    public async Task<bool> CanReduceItemQuantity(int id, int quantity)
    {
        Orderitem orderitem = await _orderItemRepository.GetOrderItemById(id);
        if ((quantity - orderitem.Readyitemquantity) >= 0)
            return true;
        return false;
    }

    public async Task<int> CompleteOrder(int id, string userId)
    {
        var order = await _orderRepository.OrderDetails(id);

        if (order.Orderitems.Count() > 0)
        {
            foreach (var item in order.Orderitems)
            {
                if (item.Quantity != item.Readyitemquantity)
                {
                    return 0;
                }
            }
        }

        await _orderRepository.UpdateOrderStatus(id, "Completed", userId);

        //update table status
        foreach (var table in order.Tableordermappings)
        {
            await _tableRepository.UpdateTableStatus((int)table.Tableid, "Available", userId);
        }

        Payment payment = new Payment
        {
            Method = "Cash",
            Amount = order.Totalamount,
            Status = "Paid",
            Createddate = DateTime.Now,
            Createdby = userId
        };
        await _paymentRepository.AddPayment(payment);

        Invoice invoice = new Invoice
        {
            Orderid = order.Id,
            Totalamount = order.Totalamount,
            Createddate = DateTime.Now,
            Createdby = userId
        };
        await _invoiceRepository.SaveInvoiceDetail(invoice);

        return 1;
    }


    public async Task<int> CancelOrder(int orderId, string userId)
    {
        try
        {
            var order = await _orderRepository.OrderDetails(orderId);
            if (order.Orderitems.Count() == 0)
            {
                await _orderRepository.UpdateOrderStatus(orderId, "Cancelled", userId);
                return 1;
            }

            //check ready items
            foreach (var item in order.Orderitems)
            {
                if (item.Readyitemquantity > 0)
                    return 0;
            }

            foreach (var table in order.Tableordermappings)
            {
                await _tableRepository.UpdateTableStatus((int)table.Tableid, "Available", userId);
            }

            await _orderRepository.UpdateOrderStatus(orderId, "Cancelled", userId);
            return 1;
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public async Task<bool> SaveCustomerFeedBack(int orderId, short? food, short? service, short? ambience, string? comment, string? userId)
    {
        Feedback feedback = new Feedback
        {
            Orderid = orderId,
            Food = food ?? 0,
            Service = service ?? 0,
            Ambience = ambience ?? 0,
            Comment = comment,
            Avgrating = Math.Round((decimal)(service + ambience + food) / 3, 2),
            Createdby = userId,
            Createddate = DateTime.Now
        };
        await _feedbackRepository.SaveCustomerFeedback(feedback);
        return true;
    }
}
