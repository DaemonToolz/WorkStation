using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using WorkstationBrowser.BLL;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Controllers.SignalR;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

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
        public async Task<ActionResult> Login(LogInModel LogModel, String returnUrl) {
            if (ModelState.IsValid)
            {
                SessionWrapper newSession = null;
                //HttpContext.Session
       
                try {

                    var Token = await TokenGeneration.FetchToken(LogModel.Username, LogModel.Password);

                    JObject deserialized = JObject.Parse(Token);
                    Token = deserialized["message"].ToString();
                    if (Token.Split('.').Length != 3)
                        throw new Exception();
                    newSession = new SessionWrapper(LogModel.Username, LogModel.Password, Token, LogModel);
                }
                catch {
                    newSession = null;
                }

                if (newSession != null && newSession.LogIn()) {
                    Session.Add("WorkstationConnection", newSession);
                    var loginClaim = new Claim(ClaimTypes.NameIdentifier, LogModel.Username);
                    var claimsIdentity = new ClaimsIdentity(
                        new[] {
                            loginClaim,
                            new Claim(ClaimTypes.Name, LogModel.Username)
                        }, 
                        
                        DefaultAuthenticationTypes.ApplicationCookie
                        );
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(claimsIdentity);

                    Session.Add("SystemNotifications", newSession.WorkstationSession.GetAllNotifications(newSession.CurrentUser.id));
                    Session.Add("CurrentUserRights", RightsReader.Decode(newSession.CurrentUser.rights));
                    Session.Add("HubInitialized", false);
                    NotificationHub.MyUsers.TryAdd(newSession.CurrentUser.username, newSession);

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
        public ActionResult Logout() {

            try {
            
                SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;

                SessionWrapper old;
                NotificationHub.MyUsers.TryRemove(currentSession.CurrentUser.username, out old);
                old = null;

                currentSession.LogOut();
            }
            catch { }
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            // Rediriger vers la page d'accueil :
            return RedirectToAction("Index", "Home");
        }
       
    }
}
