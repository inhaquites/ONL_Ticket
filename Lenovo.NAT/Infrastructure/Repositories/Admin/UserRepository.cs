using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.ViewModel.User;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Admin
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUserByFilter(UserFilter filterInfo, int pageSize = 10, int pageIndex = 0);
        Task<User> GetUser(string networkId);
        Task<User> UpdateUser(UserViewModel userViewModel);
        Task<User> GetUserDetail(Guid id);
        Task<long> CountUsersByFilter(UserFilter filterInfo);
        Task<IEnumerable<User>> GetUsers(int pageSize = 10, int pageIndex = 0);
        Task<List<Module>> GetModules();
        Task<List<UserModuleAccess>> GetUserModuleAccess(Guid id);
        Task CreateUserAccessHistory(UserAccessHistory userAccessHistory);
        Dictionary<string, int> GetPageVisits();
        string GetAllLogisticUsers();
    }
    public class UserRepository : IUserRepository
    {
        private readonly ThinkToolContext _thinkToolContext;
        public UserRepository(ThinkToolContext thinkToolContext)
        {
            _thinkToolContext = thinkToolContext;
        }

        public async Task<User> GetUser(string networkId)
        {
            return await _thinkToolContext.Users.FirstOrDefaultAsync(user => user.NetworkId == networkId) ?? new User();
        }

        public async Task<IEnumerable<User>> GetUsers(int pageSize = 10, int pageIndex = 0)
        {
            var itemsOnPage = await _thinkToolContext.Users
                .OrderBy(x => x.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return itemsOnPage;
        }

        public async Task<long> CountUsersByFilter(UserFilter filterInfo)
        {
            return await _thinkToolContext.Users
                .Where(user => (string.IsNullOrEmpty(filterInfo.NetworkId) || user.NetworkId.Contains(filterInfo.NetworkId))
                    && (string.IsNullOrEmpty(filterInfo.Name) || user.Name != null && user.Name.Contains(filterInfo.Name))
                    && (string.IsNullOrEmpty(filterInfo.Email) || user.Email.Contains(filterInfo.Email))
                    && (string.IsNullOrEmpty(filterInfo.Department) || user.Department != null && user.Department.Contains(filterInfo.Department))
                    && (string.IsNullOrEmpty(filterInfo.Position) || user.Position != null && user.Position.Contains(filterInfo.Position))
                    && (user.IsActive ? filterInfo.IsActive == true : filterInfo.IsActive == false)
                )
                .OrderBy(x => x.Name)
                .LongCountAsync();
        }

        public async Task<IEnumerable<User>> GetUserByFilter(UserFilter filterInfo, int pageSize = 10, int pageIndex = 0)
        {
            return await _thinkToolContext.Users
                .Where(user => (string.IsNullOrEmpty(filterInfo.NetworkId) || user.NetworkId.Contains(filterInfo.NetworkId))
                    && (string.IsNullOrEmpty(filterInfo.Name) || user.Name != null && user.Name.Contains(filterInfo.Name))
                    && (string.IsNullOrEmpty(filterInfo.Email) || user.Email.Contains(filterInfo.Email))
                    && (string.IsNullOrEmpty(filterInfo.Department) || user.Department != null && user.Department.Contains(filterInfo.Department))
                    && (string.IsNullOrEmpty(filterInfo.Position) || user.Position != null && user.Position.Contains(filterInfo.Position))
                    && (user.IsActive ? filterInfo.IsActive == true : filterInfo.IsActive == false)
                )
                .OrderBy(x => x.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User> GetUserDetail(Guid id)
        {
            return await _thinkToolContext.Users
                .FirstOrDefaultAsync(user => user.Id == id.ToString()) ?? new User();
        }

        public async Task<List<Module>> GetModules()
        {
            return await _thinkToolContext.Module.OrderByDescending(m => m.Id).ToListAsync();
        }

        public async Task<List<UserModuleAccess>> GetUserModuleAccess(Guid id)
        {
            return await _thinkToolContext.UserModuleAccess
                 .Where(user => user.UserId == id)
                 .ToListAsync();
        }

        public string GetAllLogisticUsers()
        {
            var networkIds = _thinkToolContext.Users
                .FromSqlRaw(@"
                    SELECT 
	                    us.Name,
	                    networkId + '@lenovo.com' as NetworkId,
	                    Position,
	                    Department,
	                    IsActive,
	                    CreatedAt,
	                    CreatedBy,
	                    UpdatedAt,
	                    UpdatedBy,
	                    CountryId
                    FROM role rl 
                        inner join UserRoles ur on ur.RoleId = rl.Id
                        inner join [User] us on ur.UserId = us.Id
                    WHERE rl.Name = 'Logistic'
                ")
                .Select(u => u.NetworkId)
                .Distinct()
                .ToList();

            return string.Join(";", networkIds); ;
        }

        public async Task<User> UpdateUser(UserViewModel userViewModel)
        {
            var result = await _thinkToolContext.Users.FirstOrDefaultAsync(user => user.Id == userViewModel.Id.ToString()) ?? new User();

            if (result != null)
            {
                result.Name = userViewModel.Name;
                result.Email = userViewModel.Email;
                result.Position = userViewModel.Position;
                result.CountryId = userViewModel.CountryID;
                result.IsActive = userViewModel.IsActive;

                _thinkToolContext.Update(result);

                await _thinkToolContext.SaveChangesAsync();

                return result;
            }

            return result;
        }

        public async Task CreateUserAccessHistory(UserAccessHistory userAccessHistory)
        {
            await _thinkToolContext.UserAccessHistory.AddAsync(userAccessHistory);
            await _thinkToolContext.SaveChangesAsync();
        }

        public Dictionary<string, int> GetPageVisits()
        {
            return _thinkToolContext.UserAccessHistory
                .GroupBy(user => user.Page)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(user => user.Id).Distinct().Count()
                );
        }
    }
}
