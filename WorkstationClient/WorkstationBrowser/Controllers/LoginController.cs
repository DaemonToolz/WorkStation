using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using WorkstationBrowser.Models;

namespace WorkstationBrowser.Controllers {
    public class LoginController : Controller {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        // POST: Login/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogInModel LogModel, String returnUrl)
        {
            if (ModelState.IsValid){
                if (true)
                {
                    
                    var loginClaim = new Claim(ClaimTypes.NameIdentifier, LogModel.Username);
                    var claimsIdentity = new ClaimsIdentity(new[] { loginClaim }, DefaultAuthenticationTypes.ApplicationCookie);
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(claimsIdentity);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(LogModel);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            // Rediriger vers la page d'accueil :
            return RedirectToAction("Index", "Home");
        }
       
    }
}
