using Newtonsoft.Json;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC.Model.UserData
{
    public class UserRedisRepository : IUserRedisRepository
    {
        private static Dictionary<string, UserCounterData>  _database 
            = new Dictionary<string, UserCounterData>();

        public UserCounterData GetUserCounterData(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException();

            _database.TryGetValue(customerId, out UserCounterData data);
            if (data == null)
            {
                List<UserNotification> userNotifications = new List<UserNotification>();
                data = new UserCounterData(userNotifications, 0);
                UpdateUserCounterData(customerId, data);
                return data;
            }

            data.Notifications = data.Notifications.OrderByDescending(n => n.DateCreated).ToList();
            return data;
        }

        public void AddUserNotification(string customerId, UserNotification userNotification)
        {
            var userCounterData = GetUserCounterData(customerId);
            userCounterData.Notifications.Add(userNotification);
            UpdateUserBasketCount(customerId, userCounterData.BasketCount);
        }

        public void UpdateUserBasketCount(string customerId, int userBasketCount)
        {
            var userCounterData = GetUserCounterData(customerId);
            userCounterData.BasketCount = userBasketCount;
            UpdateUserCounterData(customerId, userCounterData);
        }

        public void MarkAllAsRead(string customerId)
        {
            UserCounterData userCounterData = GetUserCounterData(customerId);
            foreach (var notification in
                userCounterData.Notifications.Where(n => !n.DateVisualized.HasValue))
            {
                notification.DateVisualized = DateTime.Now;
            }
            UpdateUserCounterData(customerId, userCounterData);
        }

        private void UpdateUserCounterData(string customerId, UserCounterData userCounterData)
        {
            if (userCounterData == null)
                throw new ArgumentNullException();

            if (userCounterData.BasketCount < 0)
                throw new ArgumentOutOfRangeException();

            if (_database.ContainsKey(customerId))
            {
                _database[customerId] = userCounterData;
                return;
            }
            _database.Add(customerId, userCounterData);
        }
    }
}
