using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class DepartmentController : GenericController
    {       
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            List<DepartmentHeavyModel> heavyModels = new List<DepartmentHeavyModel>();
            foreach (var model in _Session.GetAllDepartments())
            {
                var teams = _Session.GetAllTeams().Where(team => team.department_id == model.id).ToArray();
                List<UsersModel> users = new List<UsersModel>();
                foreach (var teamModel in teams)
                    users.AddRange(_Session.GetUsersByTeam(teamModel.id));

                heavyModels.Add(new DepartmentHeavyModel()
                {
                    OriginalModel = model,
                    Teams = teams,
                    Users = users.ToArray()
                });
            }

            return View(heavyModels);
        }


        public ActionResult Details(string name) {

            // This rzquires a simplification of the model / cache system

            var selectedDept = _Session.GetDepartments(name);
            var teamsPerDept = _Session.GetAllTeams().Where(team => team.department_id == selectedDept.id);
            List<UsersModel> usersPerDept = new List<UsersModel>();
            
            
            foreach (var teamModel in teamsPerDept)
                usersPerDept.AddRange(_Session.GetUsersByTeam(teamModel.id));
            

            return View(new DepartmentHeavyModel(){ OriginalModel = selectedDept, Teams = teamsPerDept.ToArray(), Users = usersPerDept.ToArray()});
        }
     
    }
}
