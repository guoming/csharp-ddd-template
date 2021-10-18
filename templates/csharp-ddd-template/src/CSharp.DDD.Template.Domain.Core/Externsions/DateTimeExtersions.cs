using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.DDD.Template.Domain.Core.Externsions
{
    public static class DateTimeExtersions
    {
        public static long ToTimestamp(this DateTimeOffset nowTime)
        {
            return (nowTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }


        public static long ToTimestamp(this DateTime nowTime)
        {
            return (nowTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
    }
}
