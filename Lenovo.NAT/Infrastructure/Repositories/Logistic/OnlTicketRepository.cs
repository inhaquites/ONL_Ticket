using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic;

public interface IOnlTicketRepository
{
    // Dropdown data
    Task<IEnumerable<OrderType>> GetAllOrderTypes();
    Task<IEnumerable<OrderStatus>> GetAllOrderStatuses();
    Task<IEnumerable<NFType>> GetAllNFTypes();
    Task<IEnumerable<CustomerSegment>> GetAllCustomerSegments();
    Task<IEnumerable<Country>> GetAllCountries();
    
    // ONL Ticket CRUD
    Task<OnlTicket?> GetOnlTicketByIdAsync(int id);
    Task<IEnumerable<OnlTicket>> GetAllOnlTicketsAsync();
    Task<OnlTicket> CreateOnlTicketAsync(OnlTicket onlTicket);
    Task<OnlTicket> UpdateOnlTicketAsync(OnlTicket onlTicket);
    Task<bool> DeleteOnlTicketAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public class OnlTicketRepository : IOnlTicketRepository
{
    private readonly ThinkToolContext _thinkToolContext;
    public OnlTicketRepository(ThinkToolContext thinkToolContext)
    {
        _thinkToolContext = thinkToolContext;    
    }

    public async Task<IEnumerable<OrderType>> GetAllOrderTypes() => await _thinkToolContext.OrderType.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<IEnumerable<OrderStatus>> GetAllOrderStatuses() => await _thinkToolContext.OrderStatus.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<IEnumerable<NFType>> GetAllNFTypes() => await _thinkToolContext.NFType.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<IEnumerable<CustomerSegment>> GetAllCustomerSegments() => await _thinkToolContext.CustomerSegment.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<IEnumerable<Country>> GetAllCountries() => await _thinkToolContext.Countries.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted).OrderBy(x => x.Name).ToListAsync();

    // ONL Ticket CRUD Methods
    public async Task<OnlTicket?> GetOnlTicketByIdAsync(int id)
    {
        return await _thinkToolContext.OnlTicket
            .Include(x => x.Attachments)
            .Include(x => x.SoldToAddresses)
                .ThenInclude(x => x.ShipToAddresses)
                    .ThenInclude(x => x.OrderItems)
            .Include(x => x.SAPOrders)
            .Include(x => x.Comments)
            .Include(x => x.CountryNavigation)
            .Include(x => x.SegmentNavigation)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OnlTicket>> GetAllOnlTicketsAsync()
    {
        return await _thinkToolContext.OnlTicket
            .Include(x => x.CountryNavigation)
            .Include(x => x.SegmentNavigation)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<OnlTicket> CreateOnlTicketAsync(OnlTicket onlTicket)
    {
        _thinkToolContext.OnlTicket.Add(onlTicket);
        await _thinkToolContext.SaveChangesAsync();
        return onlTicket;
    }

    public async Task<OnlTicket> UpdateOnlTicketAsync(OnlTicket onlTicket)
    {
        _thinkToolContext.OnlTicket.Update(onlTicket);
        await _thinkToolContext.SaveChangesAsync();
        return onlTicket;
    }

    public async Task<bool> DeleteOnlTicketAsync(int id)
    {
        var onlTicket = await _thinkToolContext.OnlTicket.FindAsync(id);
        if (onlTicket == null) return false;

        _thinkToolContext.OnlTicket.Remove(onlTicket);
        await _thinkToolContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _thinkToolContext.OnlTicket.AnyAsync(x => x.Id == id);
    }

}
