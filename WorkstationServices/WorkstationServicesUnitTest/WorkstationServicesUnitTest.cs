using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Workstation.Company.Infos;
using WorkstationServicesUnitTest.WorkstationSessionReference;

using System.Collections.Generic;
using WorkstationMessaging.Model;
using Workstation.Model;
using System.Threading;

namespace WorkstationUnitTests {
   
    [TestClass]
    public class WorkstationServicesUnitTest{
        private SessionClient client;
        private UsersModel user;

        [TestInitialize]
        public void Initialize(){
            client = new SessionClient();

            client.ClientCredentials.UserName.UserName = "DefaultAuthorizedUser";
            client.ClientCredentials.UserName.Password = "defaultpassword";
        }

        [TestMethod]
        public void TestServices(){
            TestLogin();
            TestGetAll();
            TestEditUser();
            TestLogOut();
        }

        public void TestLogin()
        {

            Assert.IsNotNull(client);
            Console.WriteLine("============================ LOGGING IN ============================");
            String Token = FetchToken("DefaultAuthorizedUser", "defaultpassword").Result;
            
            JObject deserialized = JObject.Parse(Token);
            Token = deserialized["message"].ToString();
            Console.WriteLine($"Token Received {Token}");

            user = client.LogIn("DefaultAuthorizedUser", Token);

            Console.WriteLine($"User Received {user.id}");
     
            Console.WriteLine("============================ END LOGGING IN ============================");
        }

        public void TestLogOut(){
            Console.WriteLine("============================ LOGGING OUT ============================");
            client.LogOut(user);
            Console.WriteLine("============================ END LOGGING OUT============================");

        }

        public void TestGetAll() {
            Console.WriteLine("============================ GET ALL ============================");
            Workstation.Model.ProjectModel[] projects = client.GetAllProjects();
  
            Assert.IsNotNull(projects);
            foreach (var project in projects)
                Console.WriteLine($"Project : { project.id } - { project.name } - { project.root}");

            UsersModel[] users = client.GetAllUsers();

            Assert.IsNotNull(users);
            foreach (var user in users)
                Console.WriteLine($"User : { user.id } - { user.username } - { user.email }");

            DepartmentModel[] departments = client.GetAllDepartments();
    
            Assert.IsNotNull(departments);
            foreach (var department in departments)
                Console.WriteLine($"Department : { department.id } - { department.name } ");

            TeamModel[] teams = client.GetAllTeams();
    
            Assert.IsNotNull(teams);
            foreach (var team in teams)
                Console.WriteLine($"Team : { team.id } - { team.name } - { team.project_id } - {team.department_id}");

            Console.WriteLine("============================ END GET ALL ============================");
        }

        public void TestGetOne()
        {
            Console.WriteLine("============================ GET ONE ============================");

            Console.WriteLine("============================ END GET ONE ============================");

        }


        public void TestEditUser()
        {
            Console.WriteLine("============================ EDITING USER ============================");
            var Original = client.GetAllUsers().Single(user => user.username.Equals("Test user"));

            var MyUser = client.GetAllUsers().Single(user => user.username.Equals("Test user"));

            MyUser.email = "New_Email@email.email";
            Assert.IsTrue(client.EditUser(MyUser));

            MyUser = client.GetAllUsers().Single(user => user.username.Equals("Test user"));

            Assert.IsTrue(!MyUser.email.Equals(Original.email));

            Console.WriteLine("============================ END EDITING USER ============================");

        }

        public void TestDeleteUser()
        {
            
        }

        public void TestEditProject()
        {

        }

        public void TestDeleteProject()
        {

        }


        [TestCleanup]
        public void CleanUp()
        {

        }

        private async Task<String> FetchToken(String Username, String Password){
            var client = new HttpClient();


            client.DefaultRequestHeaders.Add("User-Agent", "Workstation Probing Agent");
            client.DefaultRequestHeaders.Add("Username", Username);
            client.DefaultRequestHeaders.Add("Password", Password);

            Console.WriteLine("Fetching the Token at http://localhost:15572/api/Tokens/Generate");
            var stringTask = client.GetStringAsync("http://localhost:15572/api/Tokens/Generate");
            
            return await stringTask;// (serializer.ReadObject(await streamTask) as MessageContract);

        }
    }
}
