using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectFromJson(this ISession session,string key,object value)
        {
            session.SetString(key,JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session,string key)
        {
            var value=session.GetString(key);
            return value is null?default:JsonSerializer.Deserialize<T>(value);
        }
    }
}
