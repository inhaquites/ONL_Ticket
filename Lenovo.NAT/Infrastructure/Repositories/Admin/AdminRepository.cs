using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Admin
{
    public interface IAdminRepository
    {
        Task<List<Country>> GetCountries();
        string GetCountryName(Guid countryId);
    }
    public class AdminRepository : IAdminRepository
    {
        private readonly ThinkToolContext _thinkToolContext;
        public AdminRepository(ThinkToolContext thinkToolContext)
        {
            _thinkToolContext = thinkToolContext;
        }

        public async Task<List<Country>> GetCountries()
        {
            return await _thinkToolContext.Countries.ToListAsync();
        }
        public string GetCountryName(Guid countryId)
        {
            return _thinkToolContext.Countries.FirstOrDefault(x => x.Id == countryId)?.Name ?? string.Empty;
        }
    }
}
