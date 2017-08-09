using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Workstation.Company.Infos;

namespace WorkstationUnitTests
{
   
    [TestClass]
    public class TokenUnitTest
    {
     
        [TestInitialize]
        public void Initialize(){

        }

        [TestMethod]
        public void TestLoadCompanyInfo()
        {

            Console.WriteLine("====================== TESTING COMPANY INFO LOADING ====================== ");
            CompanyInfoUtil.LoadCompanyInfos("./CompanyInfo.json");
            Assert.IsTrue(CompanyInfoUtil.CompanyClaims.ValidIssuers.Length > 0);

            foreach (var str in CompanyInfoUtil.CompanyClaims.ValidIssuers)
                Console.WriteLine($"Issuer {str}");

            Console.WriteLine();
            Assert.IsTrue(CompanyInfoUtil.CompanyClaims.ValidAudiences.Length > 0);

            foreach (var str in CompanyInfoUtil.CompanyClaims.ValidIssuers)
                Console.WriteLine($"Adueince {str}");

            Console.WriteLine();

            Assert.IsTrue(CompanyInfoUtil.CompanyClaims.Claims.Count > 0);
            foreach (var claimKey in CompanyInfoUtil.CompanyClaims.Claims.Keys)
                Console.WriteLine($"Claim {claimKey} : {CompanyInfoUtil.CompanyClaims.Claims[claimKey]}");

            var CompanyClaims = CompanyInfoUtil.CompanyClaims.Claims;
            
            ClaimsIdentity Identity =  new ClaimsIdentity(
                CompanyClaims.Keys.ToList().Select(key => new Claim(typeof(ClaimTypes).GetField(key).GetValue(null).ToString(), CompanyClaims[key]))
                , "Custom");

            Assert.IsTrue(Identity.Claims.Any());
            foreach (var claim in Identity.Claims)
                Console.WriteLine($"Claim {claim}");


            Console.WriteLine("====================== END TESTING COMPANY INFO LOADING ====================== ");
            
        }

        private String ReceivedToken;



        [TestMethod]
        public void TestAuthenticationService()
        {
            TestTokenGeneration();
            TestTokenValidation();
        }

        public void TestTokenGeneration()
        {
            Console.WriteLine("====================== TESTING TOKEN GENERATION ====================== ");

    
            var taskResult = FetchToken("DefaultAuthorizedUser", "defaultpassword").Result;
            Console.WriteLine($"Result : {taskResult}");
            JObject deserialized = JObject.Parse(taskResult);
            ReceivedToken = deserialized["message"].ToString();
            Assert.IsNotNull(ReceivedToken);
     
            Console.WriteLine($"Token {ReceivedToken}");

            Assert.IsFalse(ReceivedToken.Contains(" "));
            
            Assert.IsTrue(ReceivedToken.Split('.').Count() == 3);
            Console.WriteLine("====================== END TESTING TOKEN GENERATION ====================== ");
        }


        public void TestTokenValidation(){
            Console.WriteLine("====================== TESTING TOKEN VALIDATION ====================== ");

            var taskResult = ValidateToken("DefaultAuthorizedUser", "defaultpassword", ReceivedToken).Result;
            Console.WriteLine($"Result : {taskResult}");

            bool result = Boolean.Parse(taskResult);
            Assert.IsTrue(result);


            Console.WriteLine("====================== END TESTING TOKEN VALIDATION ====================== ");

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

        private async Task<String> ValidateToken(String Username, String Password, String Token)
        {
            var client = new HttpClient();


            client.DefaultRequestHeaders.Add("User-Agent", "Workstation Probing Agent");
            client.DefaultRequestHeaders.Add("Username", Username);
            client.DefaultRequestHeaders.Add("Password", Password);
            client.DefaultRequestHeaders.Add("CustomAuthorizers", ReceivedToken);
            client.DefaultRequestHeaders.Add("CheckExistence", "true");

            Console.WriteLine("Check the Token at http://localhost:15572/api/Tokens/Check");
            var stringTask = client.GetStringAsync("http://localhost:15572/api/Tokens/Check");

            return await stringTask;// (serializer.ReadObject(await streamTask) as MessageContract);

        }
    }
}
