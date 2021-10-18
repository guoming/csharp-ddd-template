using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CSharp.DDD.Template.Infrastructure
{
    public class HttpContextProvider
    {
        public static IHttpContextAccessor Accessor;
        public static HttpContext Current
        {
            get
            {
                var context = Accessor.HttpContext;
                return context;
            }
        }

        public static T Resolve<T>()
        {
            return Current.Request.HttpContext.RequestServices.GetService<T>();
        }
    }
}
