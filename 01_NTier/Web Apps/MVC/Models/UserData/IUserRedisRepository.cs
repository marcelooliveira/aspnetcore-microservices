using Services.Models;
using System.Threading.Tasks;

namespace MVC.Model.UserData
{
    public interface IUserRedisRepository
    {
        UserCounterData GetUserCounterData(string customerId);
        void  AddUserNotification(string customerId, UserNotification userNotification);
        void  UpdateUserBasketCount(string customerId, int userBasketCount);
        void  MarkAllAsRead(string customerId);
    }
}