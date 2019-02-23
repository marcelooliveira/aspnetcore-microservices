using CasaDoCodigo.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Model.Redis
{
    public class UserRedisRepository : IUserRedisRepository
    {
        private const int USER_DB_INDEX = 1;
        private readonly ILogger<UserRedisRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public UserRedisRepository(ILogger<UserRedisRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase(USER_DB_INDEX);
        }

        private IServer GetServer()
        {
            var endpoints = _redis.GetEndPoints();
            return _redis.GetServer(endpoints.First());
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException();

            List<UserNotification> userNotifications;
            var data = await _database.StringGetAsync(customerId);
            if (data.IsNullOrEmpty)
            {
                userNotifications = new List<UserNotification>();
                await UpdateUserNotificationAsync(customerId, userNotifications);
                return userNotifications;
            }

            userNotifications = JsonConvert.DeserializeObject<List<UserNotification>>(data);
            userNotifications = userNotifications.OrderByDescending(n => n.DateCreated).ToList();
            return userNotifications;
        }

        public async Task<List<UserNotification>> GetUnreadUserNotificationsAsync(string customerId)
        {
            var notifications = await GetUserNotificationsAsync(customerId);
            return notifications.Where(n => !n.DateVisualized.HasValue).ToList();
        }

        public async Task AddUserNotificationAsync(string customerId, UserNotification userNotification)
        {
            var userNotifications = await GetUserNotificationsAsync(customerId);
            userNotifications.Add(userNotification);
            await UpdateUserNotificationAsync(customerId, userNotifications);
        }

        public async Task MarkAllAsReadAsync(string customerId)
        {
            var notifications = await GetUserNotificationsAsync(customerId);
            foreach (var notification in notifications.Where(n => !n.DateVisualized.HasValue))
            {
                notification.DateVisualized = DateTime.Now;
            }
            await UpdateUserNotificationAsync(customerId, notifications);
        }

        private async Task UpdateUserNotificationAsync(string customerId, List<UserNotification> userNotifications)
        {
            var json = JsonConvert.SerializeObject(userNotifications);
            await _database.StringSetAsync(customerId, json);
        }
    }
}
