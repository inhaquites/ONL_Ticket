using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOrderShipToRepository
{
    Task<OrderShipTo?> GetByIdAsync(int id);
    Task<IEnumerable<OrderShipTo>> GetByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderShipTo>> GetBySoldToIdAsync(int soldToId);
    Task<OrderShipTo> CreateAsync(OrderShipTo orderShipTo);
    Task<OrderShipTo> UpdateAsync(OrderShipTo orderShipTo);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByOrderIdAsync(long orderId);
    Task<bool> DeleteBySoldToIdAsync(int soldToId);
}

public class OrderShipToRepository : IOrderShipToRepository
{
    private readonly ThinkToolContext _context;
    
    public OrderShipToRepository(ThinkToolContext context)
    {
        _context = context;
    }

    public async Task<OrderShipTo?> GetByIdAsync(int id)
    {
        return await _context.OrderShipTo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OrderShipTo>> GetByOrderIdAsync(long orderId)
    {
        return await _context.OrderShipTo
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderShipTo>> GetBySoldToIdAsync(int soldToId)
    {
        return await _context.OrderShipTo
            .AsNoTracking()
            .Where(x => x.IdOrderSoldTo == soldToId)
            .ToListAsync();
    }

    public async Task<OrderShipTo> CreateAsync(OrderShipTo orderShipTo)
    {
        orderShipTo.CreatedOn = DateTime.UtcNow.AddHours(-3);
        
        _context.OrderShipTo.Add(orderShipTo);
        await _context.SaveChangesAsync();
        
        return orderShipTo;
    }

    public async Task<OrderShipTo> UpdateAsync(OrderShipTo orderShipTo)
    {
        _context.OrderShipTo.Update(orderShipTo);
        await _context.SaveChangesAsync();
        
        return orderShipTo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderShipTo.FindAsync(id);
        if (entity == null) return false;

        _context.OrderShipTo.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteByOrderIdAsync(long orderId)
    {
        var entities = await _context.OrderShipTo
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();

        _context.OrderShipTo.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteBySoldToIdAsync(int soldToId)
    {
        var entities = await _context.OrderShipTo
            .Where(x => x.IdOrderSoldTo == soldToId)
            .ToListAsync();

        _context.OrderShipTo.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
