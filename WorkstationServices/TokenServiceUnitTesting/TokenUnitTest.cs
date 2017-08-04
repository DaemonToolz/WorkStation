using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workstation.Company.Infos;
using System.Security.Claims;
using System.ServiceModel;
using TokenServiceUnitTesting.TokenManagementUnit;

namespace TokenServiceUnitTesting
{
   
    [TestClass]
    public class TokenUnitTest
    {
        private TokenGenerationServiceClient GenerationTester;
        private TokenValidationServiceClient ValidationTester;

        [TestInitialize]
        public void Initialize(){
            GenerationTester = new TokenGenerationServiceClient();
            GenerationTester.ClientCredentials.UserName.UserName = "DefaultAuthorizedUser";
            GenerationTester.ClientCredentials.UserName.Password = "defaultpassword";
            GenerationTester.Open();

            ValidationTester = new TokenValidationServiceClient();
            ValidationTester.Open();
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
        public void TestTokenGeneration()
        {
            Console.WriteLine("====================== TESTING TOKEN GENERATION ====================== ");

            try
            {
                ReceivedToken = GenerationTester.GenerateToken();
                Console.WriteLine($"Token {ReceivedToken}");
            }
            catch (FaultException fe){
                Console.Error.WriteLine(fe);
            }
            catch(Exception e){
                //Console.Error.WriteLine(e);
                if (!(e.InnerException is null))
                {
                    Console.Error.WriteLine(e.InnerException);
                    FaultException fe = e.InnerException as FaultException;
                    Console.Error.WriteLine(fe.Message);
                    Console.Error.WriteLine(fe.Source);
                    Console.Error.WriteLine(fe.Code);



                }
            }
            Assert.IsFalse(ReceivedToken.Contains(" "));
            Console.WriteLine("====================== END TESTING TOKEN GENERATION ====================== ");
        }

        [TestMethod]
        public void TestTokenValidation(){
            Console.WriteLine("====================== TESTING TOKEN VALIDATION ====================== ");
            Assert.IsFalse(ValidationTester.CheckToken(ReceivedToken,true));
            Console.WriteLine("====================== END TESTING TOKEN VALIDATION ====================== ");

        }


        [TestCleanup]
        public void CleanUp()
        {
            GenerationTester.Close();
            ValidationTester.Close();
        }

    }
}
