using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class KOTOrderAppService: IKOTOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public KOTOrderAppService(IOrderRepository orderRepository,ICategoryRepository categoryRepository,IOrderItemRepository orderItemRepository)
    {  
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<OrderAppKOTViewModel> GetKotData(int categoryId)
    {   
        var kotData = await GetKOTByCategoryId(0,1,"In Progress");
        return kotData;
    }

    public async Task<OrderAppKOTViewModel> GetKOTByCategoryId(int categoryId,int pageIndex,string? filterBy)
    {   
        var pagination = new PaginationViewModel{
            PageIndex = pageIndex,
            PageSize = 5,
            FilterBy = filterBy
        };
        var kotData = new OrderAppKOTViewModel();

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
        kotData.CategoryList = categoryList;

        List<Order> orders = await _orderRepository.GetKotContent(categoryId,pagination);
        int count = await _orderRepository.GetKotContentCount(categoryId,pagination);
        pagination.TotalRecord = count;
        pagination.TotalPage = (int)Math.Ceiling((double)count / pagination.PageSize);
        kotData.Pagination = pagination;

        List<OrderAppKOTContentViewModel> kotContent = orders.Select(o => new OrderAppKOTContentViewModel{
            OrderId = o.Id,
            Section = o.Tableordermappings.Select(t => t.Table.Section.Name).FirstOrDefault(),
            Tables = o.Tableordermappings.Select(t => t.Table.Name).ToList(),
            OrderDuration = DateTime.Now.Subtract((DateTime)o.Createddate).Days.ToString() + " Days " + DateTime.Now.Subtract((DateTime)o.Createddate).Hours.ToString() + " hours "+  DateTime.Now.Subtract((DateTime)o.Createddate).Minutes.ToString() + " min " + DateTime.Now.Subtract((DateTime)o.Createddate).Seconds.ToString() + " sec ",
            Status = o.Status,
            Notes = o.Notes,
            OrderItems = o.Orderitems.Select(i => new OrderItemViewModel{
                Id = i.Id,
                Menuitemid = i?.Menuitemid,
                Orderid = i?.Orderid,
                Comment = i?.Comment,
                Name = i.Name,
                Quantity = pagination.FilterBy == "In Progress" ? (short)(i.Quantity - i.Readyitemquantity) : (short)i.Readyitemquantity,
                Rate = i.Rate,
                Amount = i.Totalamount,
                Modifiers = i.Orderitemmodifiermappings.Select(m => new OrderItemModifierViewModel{
                    Id = m.Id,
                    OrderItemid = m.Orderitemid,
                    Name = m.Modifier.Name,
                    Quantity = m.Modifier.Quantity,
                    Rate = m.Modifier.Rate,
                    TotalAmount = m.Totalamount
                }).ToList(),
            }).ToList()
        }).ToList();

        kotData.ListKotContent = kotContent;

        return kotData;
    }


    public async Task<bool> MarkOrderItemStatus(List<OrderItemViewModel> data,string userId, string filterBy)
    {
        foreach (var item in data)
        {
            var result = await _orderItemRepository.MarkOrderItemStatus(item,userId,filterBy);
            if (!result)
            {
                return false;
            }
        }
        return true;
    }
}
