using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly PizzashopContext _context;
    public OrderItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<bool> MarkOrderItemStatus(OrderItemViewModel data, string userId, string filterBy)
    {
        var orderItem = _context.Orderitems.FirstOrDefault(x => x.Id == data.Id);
        if (orderItem != null)
        {
            orderItem.Readyitemquantity = filterBy == "In Progress" ? (short)(orderItem.Readyitemquantity + data.Quantity) : (short)(orderItem.Readyitemquantity - data.Quantity);

            if (orderItem.Readyitemquantity == orderItem.Quantity)
            {
                orderItem.Status = "Ready";
            }
            if (orderItem.Readyitemquantity == 0)
            {
                orderItem.Status = "In Progress";
            }
            orderItem.Updatedby = userId;
            orderItem.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<Orderitem> GetOrderItemById(int id)
    {
        var orderItem = await _context.Orderitems.FirstOrDefaultAsync(i => i.Id == id && i.Isdeleted == false);
        return orderItem;
    }

    public async Task<List<Orderitem>> GetOrderItemsByOrderId(int orderId)
    {
        var orderItems = await _context.Orderitems.Where(x => x.Orderid == orderId && x.Isdeleted == false).ToListAsync();
        return orderItems;
    }

    public async Task AddNewOrderItem(Orderitem orderitem)
    {
        await _context.Orderitems.AddAsync(orderitem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderItem(OrderItemViewModel orderitem, string userId)
    {
        Orderitem updatedOrderItem = await _context.Orderitems.FirstOrDefaultAsync(i => i.Id == orderitem.Id);

        updatedOrderItem.Quantity = orderitem.Quantity;
        updatedOrderItem.Amount = (decimal)orderitem.Amount;
        updatedOrderItem.Totalamount = (decimal)orderitem.TotalAmount;
        updatedOrderItem.Comment = orderitem.Comment;
        updatedOrderItem.Instruction = orderitem.Instruction;
        updatedOrderItem.Totalmodifieramount = (decimal)orderitem.TotalModifierAmount;
        updatedOrderItem.Tax = 0;
        updatedOrderItem.Updatedby = userId;
        updatedOrderItem.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderItem(int id, string userId)
    {
        var orderItem = await _context.Orderitems.FirstOrDefaultAsync(x => x.Id == id);
        if (orderItem != null)
        {
            orderItem.Isdeleted = true;
            orderItem.Updatedby = userId;
            orderItem.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SaveComment(int id, string comment, string? userId)
    {
        Orderitem orderItem = await _context.Orderitems.FirstOrDefaultAsync(i => i.Id == id && i.Isdeleted == false);
        if (orderItem != null)
        {
            orderItem.Comment = comment.Trim();
            orderItem.Updatedby = userId;
            orderItem.Updateddate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
