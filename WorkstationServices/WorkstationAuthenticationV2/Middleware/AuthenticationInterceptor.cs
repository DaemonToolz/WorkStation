using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WorkstationAuthenticationV2.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticationInterceptor
    {
        private readonly RequestDelegate _next;

        public AuthenticationInterceptor(RequestDelegate next) {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext){
          
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationInterceptorExtensions
    {
        public static IApplicationBuilder UseAuthenticationInterceptor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationInterceptor>();
        }
    }
}
