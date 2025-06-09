using System.Collections.Concurrent;
using CanteenManage.Models;

namespace CanteenManage.Services
{
    public class SessionManager
    {
        private ConcurrentBag<SessionUser> _sessionUsers;
        public SessionManager()
        {
            _sessionUsers = new ConcurrentBag<SessionUser>();
        }
        public void AddOrLoginSessionUser(string userEmpID, string Portal_Token)
        {
            var user = _sessionUsers.FirstOrDefault(u => u.UserEmpID == userEmpID);
            if (user != null)
            {
                user.isLogin = true;
            }
            else
            {
                user = new SessionUser
                {
                    UserEmpID = userEmpID,
                    Portal_Token = Portal_Token,
                    isLogin = true
                };
                _sessionUsers.Add(user);
            }
        }
        public void RemoveSessionUser(string userEmpID)
        {
            var user = _sessionUsers.FirstOrDefault(u => u.UserEmpID == userEmpID);
            var userToRemove = new SessionUser();
            if (user != null)
            {
                _sessionUsers.TryTake(out userToRemove);
            }
        }
        public SessionUser? GetSessionUser(string userEmpID)
        {
            return _sessionUsers.FirstOrDefault(u => u.UserEmpID == userEmpID);
        }
        public List<SessionUser> GetAllSessionUsers()
        {
            return _sessionUsers.ToList();
        }
        public bool IsUserLoggedIn(string userEmpID)
        {
            return _sessionUsers.Any(u => u.UserEmpID == userEmpID && u.isLogin);
        }
        public void LogoutSessionUser(string userEmpID)
        {
            var user = _sessionUsers.FirstOrDefault(u => u.UserEmpID == userEmpID);
            if (user != null)
            {
                user.isLogin = false;
            }
        }
        public void ClearAllSessions()
        {
            _sessionUsers.Clear();
        }

    }
}
