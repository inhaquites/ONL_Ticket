using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.Infrastructure.Repositories.Admin;

namespace Lenovo.NAT.Services.Admin
{
    public interface IAdminService
    {
        Task<List<Country>> GetCountries();
        string GetCountryName(Guid countryId);
    }
    public class AdminService: IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
       
        public async Task<List<Country>> GetCountries()
        {
            var countries = await _adminRepository.GetCountries();

            return countries;
        }

        public string GetCountryName(Guid countryId)
        {
            return _adminRepository.GetCountryName(countryId);
        }
    }
}
