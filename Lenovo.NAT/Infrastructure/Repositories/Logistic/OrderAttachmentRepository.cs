using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOrderAttachmentRepository
{
    Task<OrderAttachment?> GetByIdAsync(int id);
    Task<IEnumerable<OrderAttachment>> GetByOrderIdAsync(long orderId);
    Task<OrderAttachment> CreateAsync(OrderAttachment orderAttachment);
    Task<OrderAttachment> UpdateAsync(OrderAttachment orderAttachment);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByOrderIdAsync(long orderId);
}

public class OrderAttachmentRepository : IOrderAttachmentRepository
{
    private readonly ThinkToolContext _context;
    
    public OrderAttachmentRepository(ThinkToolContext context)
    {
        _context = context;
    }

    public async Task<OrderAttachment?> GetByIdAsync(int id)
    {
        return await _context.OrderAttachment
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OrderAttachment>> GetByOrderIdAsync(long orderId)
    {
        return await _context.OrderAttachment
            .AsNoTracking()
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();
    }

    public async Task<OrderAttachment> CreateAsync(OrderAttachment orderAttachment)
    {
        orderAttachment.CreatedOn = DateTime.UtcNow.AddHours(-3);
        
        _context.OrderAttachment.Add(orderAttachment);
        await _context.SaveChangesAsync();
        
        return orderAttachment;
    }

    public async Task<OrderAttachment> UpdateAsync(OrderAttachment orderAttachment)
    {
        orderAttachment.UpdatedDate = DateTime.UtcNow.AddHours(-3);
        
        _context.OrderAttachment.Update(orderAttachment);
        await _context.SaveChangesAsync();
        
        return orderAttachment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderAttachment.FindAsync(id);
        if (entity == null) return false;

        _context.OrderAttachment.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteByOrderIdAsync(long orderId)
    {
        var entities = await _context.OrderAttachment
            .Where(x => x.IdOrderNotLoaded == orderId)
            .ToListAsync();

        _context.OrderAttachment.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
