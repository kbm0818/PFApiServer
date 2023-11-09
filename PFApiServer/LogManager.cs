using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace PFApiServer
{
    public static class LogManager
    {
        public enum EventType : int
        {
            CreateAccount = 101,
            Login = 201,
            LoginAddRedis = 202,
        }

        private static ILoggerFactory? s_loggerFactory;
        private static readonly Dictionary<EventType, EventId> dictionary = new()
        {
            { EventType.CreateAccount, new EventId((int)EventType.CreateAccount, "CreateAccount") },
            { EventType.Login, new EventId((int)EventType.Login, "Login") },
            { EventType.LoginAddRedis, new EventId((int)EventType.LoginAddRedis, "LoginAddRedis") },
        };
        public static Dictionary<EventType, EventId> EventIdDic = dictionary;
        public static ILogger? Logger { get; private set; }

        public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
        {
            s_loggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(categoryName);
        }

        public static ILogger<T> GetLogger<T>() where T : class
        {
            return s_loggerFactory!.CreateLogger<T>();
        }
    }
}
