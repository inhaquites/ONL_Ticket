using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOrderNotLoadedRepository
{
    // Dropdown data (mesmo que OnlTicketRepository)
    Task<IEnumerable<OrderType>> GetAllOrderTypes();
    Task<IEnumerable<OrderStatus>> GetAllOrderStatuses();
    Task<IEnumerable<NFType>> GetAllNFTypes();
    Task<IEnumerable<CustomerSegment>> GetAllCustomerSegments();
    Task<IEnumerable<Country>> GetAllCountries();
    
    // OrderNotLoaded CRUD
    Task<OrderNotLoaded?> GetOrderNotLoadedByIdAsync(long id);
    Task<IEnumerable<OrderNotLoaded>> GetAllOrderNotLoadedAsync();
    Task<OrderNotLoaded> CreateOrderNotLoadedAsync(OrderNotLoaded orderNotLoaded);
    Task<OrderNotLoaded> UpdateOrderNotLoadedAsync(OrderNotLoaded orderNotLoaded);
    Task<bool> DeleteOrderNotLoadedAsync(long id);
    Task<bool> ExistsAsync(long id);
    
    // Métodos para entidades relacionadas
    Task<IEnumerable<OrderSoldTO>> GetSoldToByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderShipTo>> GetShipToByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderProduct>> GetProductsByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderAttachment>> GetAttachmentsByOrderIdAsync(long orderId);
}

public class OrderNotLoadedRepository : IOrderNotLoadedRepository
{
    private readonly ThinkToolContext _thinkToolContext;
    
    public OrderNotLoadedRepository(ThinkToolContext thinkToolContext)
    {
        _thinkToolContext = thinkToolContext;    
    }

    // Dropdown data (mesmo que OnlTicketRepository)
    public async Task<IEnumerable<OrderType>> GetAllOrderTypes() => 
        await _thinkToolContext.OrderType.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    
    public async Task<IEnumerable<OrderStatus>> GetAllOrderStatuses() => 
        await _thinkToolContext.OrderStatus.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    
    public async Task<IEnumerable<NFType>> GetAllNFTypes() => 
        await _thinkToolContext.NFType.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    
    public async Task<IEnumerable<CustomerSegment>> GetAllCustomerSegments() => 
        await _thinkToolContext.CustomerSegment.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    
    public async Task<IEnumerable<Country>> GetAllCountries() => 
        await _thinkToolContext.Countries.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted).OrderBy(x => x.Name).ToListAsync();

    // OrderNotLoaded CRUD Methods
    public async Task<OrderNotLoaded?> GetOrderNotLoadedByIdAsync(long id)
    {
        return await _thinkToolContext.OrderNotLoaded
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OrderNotLoaded>> GetAllOrderNotLoadedAsync()
    {
        return await _thinkToolContext.OrderNotLoaded
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();
    }

    public async Task<OrderNotLoaded> CreateOrderNotLoadedAsync(OrderNotLoaded orderNotLoaded)
    {
        orderNotLoaded.CreatedOn = DateTime.UtcNow.AddHours(-3);
        orderNotLoaded.UpdatedOn = DateTime.UtcNow.AddHours(-3);
        
        _thinkToolContext.OrderNotLoaded.Add(orderNotLoaded);
        await _thinkToolContext.SaveChangesAsync();
        
        return orderNotLoaded;
    }

    public async Task<OrderNotLoaded> UpdateOrderNotLoadedAsync(OrderNotLoaded orderNotLoaded)
    {
        orderNotLoaded.UpdatedOn = DateTime.UtcNow.AddHours(-3);
        
        _thinkToolContext.OrderNotLoaded.Update(orderNotLoaded);
        await _thinkToolContext.SaveChangesAsync();
        
        return orderNotLoaded;
    }

    public async Task<bool> DeleteOrderNotLoadedAsync(long id)
    {
        var orderNotLoaded = await _thinkToolContext.OrderNotLoaded.FindAsync(id);
        if (orderNotLoaded == null) return false;

        _thinkToolContext.OrderNotLoaded.Remove(orderNotLoaded);
        await _thinkToolContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _thinkToolContext.OrderNotLoaded.AnyAsync(x => x.Id == id);
    }

    // Métodos para entidades relacionadas
    public async Task<IEnumerable<OrderSoldTO>> GetSoldToByOrderIdAsync(long orderId)
    {
        return await _thinkToolContext.OrderSoldTO
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderShipTo>> GetShipToByOrderIdAsync(long orderId)
    {
        return await _thinkToolContext.OrderShipTo
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderProduct>> GetProductsByOrderIdAsync(long orderId)
    {
        return await _thinkToolContext.OrderProduct
            .AsNoTracking()
            .Where(p => _thinkToolContext.OrderShipTo
                .Where(s => s.IdOrderNotLoaded == orderId)
                .Select(s => s.Id)
                .Contains(p.IdOrderShipTo))
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderAttachment>> GetAttachmentsByOrderIdAsync(long orderId)
    {
        return await _thinkToolContext.OrderAttachment
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }
}
