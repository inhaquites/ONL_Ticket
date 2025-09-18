using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOrderProductRepository
{
    Task<OrderProduct?> GetByIdAsync(int id);
    Task<IEnumerable<OrderProduct>> GetByShipToIdAsync(int shipToId);
    Task<IEnumerable<OrderProduct>> GetByOrderIdAsync(long orderId);
    Task<OrderProduct> CreateAsync(OrderProduct orderProduct);
    Task<OrderProduct> UpdateAsync(OrderProduct orderProduct);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByShipToIdAsync(int shipToId);
    Task<bool> DeleteByOrderIdAsync(long orderId);
}

public class OrderProductRepository : IOrderProductRepository
{
    private readonly ThinkToolContext _context;
    
    public OrderProductRepository(ThinkToolContext context)
    {
        _context = context;
    }

    public async Task<OrderProduct?> GetByIdAsync(int id)
    {
        return await _context.OrderProduct
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OrderProduct>> GetByShipToIdAsync(int shipToId)
    {
        return await _context.OrderProduct
            .AsNoTracking()
            .Where(x => x.IdOrderShipTo == shipToId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderProduct>> GetByOrderIdAsync(long orderId)
    {
        return await _context.OrderProduct
            .AsNoTracking()
            .Where(p => _context.OrderShipTo
                .Where(s => s.IdOrderNotLoaded == orderId)
                .Select(s => s.Id)
                .Contains(p.IdOrderShipTo))
            .ToListAsync();
    }

    public async Task<OrderProduct> CreateAsync(OrderProduct orderProduct)
    {
        orderProduct.CreatedOn = DateTime.UtcNow.AddHours(-3);
        
        _context.OrderProduct.Add(orderProduct);
        await _context.SaveChangesAsync();
        
        return orderProduct;
    }

    public async Task<OrderProduct> UpdateAsync(OrderProduct orderProduct)
    {
        _context.OrderProduct.Update(orderProduct);
        await _context.SaveChangesAsync();
        
        return orderProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderProduct.FindAsync(id);
        if (entity == null) return false;

        _context.OrderProduct.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteByShipToIdAsync(int shipToId)
    {
        var entities = await _context.OrderProduct
            .Where(x => x.IdOrderShipTo == shipToId)
            .ToListAsync();

        _context.OrderProduct.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteByOrderIdAsync(long orderId)
    {
        var shipToIds = await _context.OrderShipTo
            .Where(s => s.IdOrderNotLoaded == orderId)
            .Select(s => s.Id)
            .ToListAsync();

        var entities = await _context.OrderProduct
            .Where(p => shipToIds.Contains(p.IdOrderShipTo))
            .ToListAsync();

        _context.OrderProduct.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
