using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOrderSoldTORepository
{
    Task<OrderSoldTO?> GetByIdAsync(int id);
    Task<IEnumerable<OrderSoldTO>> GetByOrderIdAsync(long orderId);
    Task<OrderSoldTO> CreateAsync(OrderSoldTO orderSoldTO);
    Task<OrderSoldTO> UpdateAsync(OrderSoldTO orderSoldTO);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByOrderIdAsync(long orderId);
}

public class OrderSoldTORepository : IOrderSoldTORepository
{
    private readonly ThinkToolContext _context;
    
    public OrderSoldTORepository(ThinkToolContext context)
    {
        _context = context;
    }

    public async Task<OrderSoldTO?> GetByIdAsync(int id)
    {
        return await _context.OrderSoldTO
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OrderSoldTO>> GetByOrderIdAsync(long orderId)
    {
        return await _context.OrderSoldTO
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }

    public async Task<OrderSoldTO> CreateAsync(OrderSoldTO orderSoldTO)
    {
        orderSoldTO.CreatedOn = DateTime.UtcNow.AddHours(-3);
        
        _context.OrderSoldTO.Add(orderSoldTO);
        await _context.SaveChangesAsync();
        
        return orderSoldTO;
    }

    public async Task<OrderSoldTO> UpdateAsync(OrderSoldTO orderSoldTO)
    {
        _context.OrderSoldTO.Update(orderSoldTO);
        await _context.SaveChangesAsync();
        
        return orderSoldTO;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderSoldTO.FindAsync(id);
        if (entity == null) return false;

        _context.OrderSoldTO.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteByOrderIdAsync(long orderId)
    {
        var entities = await _context.OrderSoldTO
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();

        _context.OrderSoldTO.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
