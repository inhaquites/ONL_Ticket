using System.Collections.Concurrent;

namespace Lenovo.NAT.Services.Admin
{
    public interface IUserSessionService
    {
        int GetOnlineUserCount();
        List<(string UserName, DateTime LastActive)> GetOnlineUsers();
    }

    public class UserSessionService : IUserSessionService
    {
        public int GetOnlineUserCount()
        {
            return UserTrackingMiddleware.GetOnlineUserCount();
        }

        public List<(string UserName, DateTime LastActive)> GetOnlineUsers()
        {
            return UserTrackingMiddleware.GetOnlineUsers();
        }
    }

    public class UserTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (DateTime LastActive, string UserName)> _userSessions = new();

        public UserTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.Connection.RemoteIpAddress.ToString();
            var path = context.Request.Path.ToString();
            var userName = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";

            _userSessions[userId] = (DateTime.UtcNow, userName);

            var expirationTime = DateTime.UtcNow.AddMinutes(-15);
            var expiredSessions = _userSessions.Where(kvp => kvp.Value.LastActive < expirationTime).Select(kvp => kvp.Key).ToList();
            foreach (var expiredSession in expiredSessions)
            {
                _userSessions.TryRemove(expiredSession, out _);
            }

            await _next(context);
        }

        public static int GetOnlineUserCount()
        {
            return _userSessions.Count;
        }

        public static List<(string UserName, DateTime LastActive)> GetOnlineUsers()
        {
            return _userSessions.Select(kvp => (kvp.Value.UserName, kvp.Value.LastActive)).ToList();
        }
    }
}
