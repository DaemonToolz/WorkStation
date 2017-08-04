using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace WorkstationAuthenticationV2.Results {
    public class AuthenticationResult{
        public Int32 Status { get; set; }
        public Object FriendlyResult { get; set; }
    }

    public class AuthenticationFilterResult : IActionResult {
        private readonly AuthenticationResult _internalResult;

        public AuthenticationFilterResult(AuthenticationResult result) {
            _internalResult = result;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_internalResult.FriendlyResult) {
                StatusCode = _internalResult.Status
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
