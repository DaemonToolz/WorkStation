using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkstationAuthenticationV2.Database.Context;
using WorkstationAuthenticationV2.Results;

namespace WorkstationAuthenticationV2.Middleware
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        //private readonly UserContext _userEf;

        public AuthenticationFilter(/*UserContext userEf*/)
        {
            //_userEf = userEf;
        }


        public override void OnActionExecuted(ActionExecutedContext context)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AuthenticationResult Result = null;
            if (context.HttpContext.Request.Headers.Keys.Contains("Username") &&
                context.HttpContext.Request.Headers.Keys.Contains("Password")) {

                String user = context.HttpContext.Request.Headers["Username"],
                    password = context.HttpContext.Request.Headers["Password"];
                //if(_userEf is null)
                using (var _userEf = new UserContext()) {
                    if (!_userEf.Users.Any(
                        obj => obj.Username.Equals(user) &&
                               obj.Password.Equals(password)))
                        Result = new AuthenticationResult(){
                            Status = StatusCodes.Status404NotFound,
                            FriendlyResult = new  { Message=$"Cannot find any user {user} with the password {password}" }
                        };
                }
            }
            else
            {
                Result = new AuthenticationResult(){
                    Status = StatusCodes.Status403Forbidden,
                    FriendlyResult = new { Message = $"No Credentials provided" } 
                };
            }

            if (Result != null)
                context.Result = new AuthenticationFilterResult(Result);
            base.OnActionExecuting(context);
        }
    }
}
