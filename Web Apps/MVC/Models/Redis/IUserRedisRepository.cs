using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Model.Redis
{
    public interface IUserRedisRepository
    {
        Task<List<UserNotification>> GetUserNotificationsAsync(string customerId);
        Task<List<UserNotification>> GetUnreadUserNotificationsAsync(string customerId);
        Task AddUserNotificationAsync(string customerId, UserNotification userNotification);
        Task MarkAllAsReadAsync(string customerId);
    }
}