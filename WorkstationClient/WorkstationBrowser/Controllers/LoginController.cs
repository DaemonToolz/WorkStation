﻿using System;
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
using WorkstationBrowser.BLL.Security;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Controllers.SignalR;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers {
    public class LoginController : GenericController
    {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        // POST: Login/Delete/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AntiThrottleSystem(Name = "LogIn", Message = "You have performed this action more than {x} times in the last {n} seconds.", Requests = 3, Seconds = 60)]
        public async Task<ActionResult> Login(LogInModel LogModel, String returnUrl) {
            if (!ModelState.IsValid) return View(LogModel);

            SessionWrapper newSession = null;

            try {

                var Token = await TokenGeneration.FetchToken(LogModel.Username, LogModel.Password);

                Token = (JObject.Parse(Token))["message"].ToString();


                if (Token.Split('.').Length != 3)
                    throw new Exception();

                newSession = new SessionWrapper(LogModel.Username, LogModel.Password, Token, LogModel, Session);
            }
            catch {
                newSession = null;
            }

            if (newSession != null && newSession.LogIn()) {
                _Session = newSession;
               
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

                _UserNotifications = newSession.WorkstationSession.GetAllNotifications((int)newSession.CurrentUser.id);
                _UserRights = RightsReader.Decode(newSession.CurrentUser.rights) as Dictionary<String,bool>;

                Session.Add("HubInitialized", false);
                NotificationHub.MyUsers.TryAdd(newSession.CurrentUser.username, newSession);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Either the username or the password is incorrect!");
            }
            return View(LogModel);
        }

        [HttpGet]
        public ActionResult Logout() {

            try {
            
                NotificationHub.MyUsers.TryRemove(_Session.CurrentUser.username, out SessionWrapper old);
                _Session.LogOut();
            }
            catch
            {
                // ignored
            }

            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            _Session = null;
            // Rediriger vers la page d'accueil :
            return RedirectToAction("Index", "Home");
        }
       
    }
}
