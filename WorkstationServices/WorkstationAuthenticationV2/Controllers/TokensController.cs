using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Workstation.Authentication.Security;
using WorkstationAuthenticationV2.Database.Context;
using WorkstationAuthenticationV2.Middleware;
using WorkstationAuthenticationV2.Models;
using Workstation.Company.Infos;

namespace WorkstationAuthenticationV2.Controllers
{
    [Produces("application/json")]
    [Route("api/Tokens")]
    [AuthenticationFilter]
    public class TokensController : Controller
    {
        #region 
        private readonly TokensContext _context;
        #endregion

        #region Common fields
        private static readonly IdGenerator IdGenerationModel;
        private static readonly Int32 _ID_GEN_SIZE = (int) TokenManagementUtil.MediumIdSize; // By default, the size of key is 16b
        private static readonly ClaimsIdentity Claims;

        private static readonly System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler TokenHandler =
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        #endregion

        static TokensController()
        {

            // Problem using Interpolated Strings
            // Version used C# 7.0, detected C# 5.0
            // Loading the information file
            CompanyInfoUtil.LoadCompanyInfos(String.Format(@"{0}\Infos\CompanyInfo.json", Startup.ApplicationBasePath));

            // Common claims
            Claims = new ClaimsIdentity(
                CompanyInfoUtil.CompanyClaims.Claims.Keys.ToList()
                    .Select(key => new Claim(key, CompanyInfoUtil.CompanyClaims.Claims[key])).ToList<Claim>()
                , "Custom");

            
            IdGenerationModel = new IdGenerator(1);

            // Warmup to avoid false positives
            for (int i = 0; i < 2; ++i) IdGenerationModel.GenerateId(_ID_GEN_SIZE);

        }

        public TokensController(TokensContext context)
        {
            _context = context;
        }

        // GET: api/Tokens
     
        private bool TokenExists(string id)
        {
            return _context.Token.Any(e => e.Jni == id);
        }




        /// <summary>
        /// Vérifie que le token est valide
        /// </summary>
        /// <param name="input"> Final token </param>
        /// <param name="stoken"> Input token </param>
        /// <returns></returns>
        private bool IsValidToken(string input, SecurityToken stoken)
        {
            //
            Token token = _context.Token.Single(tken => tken.Token1.Equals(input)); 

            //
            if (token.Exp != null) {
                if (DateTime.Now > token.Exp) {
                    // If expired, removal + invalid
                    _context.Token.Remove(token);
                    _context.SaveChanges();
                    return false;
                }
                else
                {

                    var tokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudiences = CompanyInfoUtil.CompanyClaims.ValidAudiences,
                        ValidIssuers = CompanyInfoUtil.CompanyClaims.ValidIssuers,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Jni))
                    };

                    string realToken = input.Substring(0, input.Length - 12);
                    SecurityToken Validated;

                    // Generate a new token plus comparing both
                    TokenHandler.ValidateToken(realToken, tokenValidationParameters, out Validated);

                    return token.Boundmac.Equals(Validated.ToString());
                }
            }
            else
                return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomAuthorizers"> Security Token </param>
        /// <param name="CheckExistence"> Force to check whether or not the validity of the token </param>
        /// <returns></returns>
        [HttpGet("Check")]
        public bool CheckToken([FromHeader] string CustomAuthorizers, [FromHeader] string CheckExistence = "false")
        {
            //try
            //{
            bool checkExistence = Boolean.Parse(CheckExistence); 
            string realToken = CustomAuthorizers.Substring(0, CustomAuthorizers.Length - 12); // Retrieve the true token
            string salt = CustomAuthorizers.Substring(CustomAuthorizers.Length - 12); // Retrieve the salt
            if (TokenHandler.CanReadToken(realToken)){
                // Check if either JWT token or simple token
                SecurityToken stoken;
                try {
                    stoken = TokenHandler.ReadJwtToken(realToken);
                } catch {
                    stoken = TokenHandler.ReadToken(realToken);
                }

                if (checkExistence && TokenExists(CustomAuthorizers))
                    if (IsValidToken(CustomAuthorizers, stoken)) return true ;
                    else return false;
                return true;
            }
            return false;

            /*}
            catch {
                return false ;
            }*/
            //return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Generate")]
        public Object GenerateToken(){
            try{
                bool changes = false;

                String jni = IdGenerationModel.GenerateId(_ID_GEN_SIZE);
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jni));
                signingKey.KeyId = IdGenerationModel.GenerateId(24);
                var signingCredentials =
                    new SigningCredentials(signingKey,
                        SecurityAlgorithms
                            .HmacSha256); // SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

                // Descriptor
                DateTime CallDate = DateTime.Now, ExpirationDate = CallDate.AddMinutes(10.0d);
                var securityTokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {

                    Issuer = CompanyInfoUtil.CompanyClaims.ValidIssuers[0],
                    IssuedAt = CallDate,
                    Expires = ExpirationDate,
                    Audience = CompanyInfoUtil.CompanyClaims.ValidAudiences[0],

                    Subject = Claims,
                    SigningCredentials = signingCredentials,
                };


                // Encoded token
                var plainToken = TokenHandler.CreateToken(securityTokenDescriptor);
                var signedAndEncodedToken = TokenHandler.WriteToken(plainToken);

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudiences = CompanyInfoUtil.CompanyClaims.ValidAudiences,
                    ValidIssuers = CompanyInfoUtil.CompanyClaims.ValidIssuers,
                    RequireExpirationTime = true,
                    IssuerSigningKey = signingKey
                };

                SecurityToken validatedToken;
                TokenHandler.ValidateToken(signedAndEncodedToken, tokenValidationParameters, out validatedToken);

                signedAndEncodedToken += IdGenerationModel.GenerateId(12);
 
                
                if (!TokenExists(signedAndEncodedToken) && CheckToken(signedAndEncodedToken))
                {
                    //var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
                    _context.Token.Add(new Token()
                    {
                        Jni = jni,
                        TKey = signingKey.KeyId.ToString(),
                        Token1 = signedAndEncodedToken,
                        Beg = CallDate,
                        Exp = ExpirationDate,

                        Boundmac = validatedToken.ToString() // To rename
                    });
                    changes = true;
                }

                if (TokenExists(signedAndEncodedToken)) {
                    _context.Token.Remove(
                        _context.Token.First(token => token.Token1.Equals(signedAndEncodedToken)));
                    changes = true;
                }

                if (changes)
                    _context.SaveChanges();

                return new { Message = signedAndEncodedToken};
            }
            catch (Exception e)
            {
                return new { Message = $"An error ocurred during the validation process {e}" };
            }
        }
    }
}