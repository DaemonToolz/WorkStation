using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly TokensContext _context;
        private static readonly TokensContext _GlobalContext = new TokensContext();

        private static readonly IdGenerator IdGenerationModel;
        private static readonly Int32 _ID_GEN_SIZE = (int) TokenManagementUtil.MediumIdSize;
        private static readonly ClaimsIdentity Claims;

        private static readonly System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler TokenHandler =
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        private static readonly Timer TokenVerificationTrigger;


        private readonly String MessageTemplate = "{\"Message\":{0}}";
        static TokensController()
        {

            // Problem using Interpolated Strings
            // Version used C# 7.0, detected C# 5.0
            CompanyInfoUtil.LoadCompanyInfos(String.Format(@"{0}\Infos\CompanyInfo.json", Startup.ApplicationBasePath));

            // Claims commun à tous les tokens
            Claims = new ClaimsIdentity(
                CompanyInfoUtil.CompanyClaims.Claims.Keys.ToList()
                    .Select(key => new Claim(key, CompanyInfoUtil.CompanyClaims.Claims[key])).ToList<Claim>()
                , "Custom");

            IdGenerationModel = new IdGenerator(1);
            for (int i = 0; i < 2; ++i) IdGenerationModel.GenerateId(_ID_GEN_SIZE);

            // Nettoyage des tokens expirés
            TokenVerificationTrigger = new Timer((_ => CheckTokenStates()), null, 2000, (60 * 1000));

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



        private static void CheckTokenStates()
        {
            try
            {
                _GlobalContext.Token.RemoveRange(
                    _GlobalContext.Token.Where(token => token.Exp != null && token.Exp < DateTime.Now));
                _GlobalContext.SaveChanges();
            }
            catch
            {

            }

        }


        /// <summary>
        /// Vérifie que le token est valide
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stoken"></param>
        /// <returns></returns>
        private static bool IsValidToken(string input, SecurityToken stoken)
        {
            Token token = _GlobalContext.Token.Single(tken => tken.Token1.Equals(input));

            //
            if (token.Exp != null)
            {
                if (DateTime.Now > token.Exp)
                {
                    // Si expiré
                    _GlobalContext.Token.Remove(token);
                    _GlobalContext.SaveChanges();
                    return false;
                }
                else
                {

                    // Paramtères de validation
                    var tokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudiences = CompanyInfoUtil.CompanyClaims.ValidAudiences,
                        ValidIssuers = CompanyInfoUtil.CompanyClaims.ValidIssuers,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Jni))
                    };

                    string realToken = input.Substring(0, input.Length - 12);
                    SecurityToken Validated;
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
        /// <param name="CustomAuthorizers"></param>
        /// <param name="CheckExistence"></param>
        /// <returns></returns>
        [HttpGet("Check")]
        public bool CheckToken([FromHeader] string CustomAuthorizers, [FromHeader] bool CheckExistence = false)
        {
            try
            {
                string realToken = CustomAuthorizers.Substring(0, CustomAuthorizers.Length - 12); // On récupère le vrai token
                string salt = CustomAuthorizers.Substring(CustomAuthorizers.Length - 12); // Les 12 derniers caractères
                if (TokenHandler.CanReadToken(realToken))
                {
                    SecurityToken stoken;
                    try
                    {
                        stoken = TokenHandler.ReadJwtToken(realToken);
                    }
                    catch
                    {
                        stoken = TokenHandler.ReadToken(realToken);
                    }

                    if (CheckExistence && TokenExists(CustomAuthorizers))
                        if (IsValidToken(CustomAuthorizers, stoken)) return true ;
                        else return false;
                    
                    return true;

                }
                return false;

            }
            catch
            {

                return false ;

            }
            //return false;
        }

        /// <summary>
        /// Génère le token de connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("Generate")]
        public Object GenerateToken(){
            try{
                bool changes = false;

                // Informations du token
                String jni = IdGenerationModel.GenerateId(_ID_GEN_SIZE);
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jni));
                signingKey.KeyId = IdGenerationModel.GenerateId(24);
                var signingCredentials =
                    new SigningCredentials(signingKey,
                        SecurityAlgorithms
                            .HmacSha256); // SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

                // Descripteur
                DateTime CallDate = DateTime.Now, ExpirationDate = CallDate.AddMinutes(5.0d);
                var securityTokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {

                    Issuer = CompanyInfoUtil.CompanyClaims.ValidIssuers[0],
                    IssuedAt = CallDate,
                    Expires = ExpirationDate,
                    Audience = CompanyInfoUtil.CompanyClaims.ValidAudiences[0],

                    Subject = Claims,
                    SigningCredentials = signingCredentials,
                };


                // Token encodé
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
 
                   // .GetType().GetProperty("Foo")?.GetValue(o, null);
                // Vérifie le token (existe ou non)
                
                if (!TokenExists(signedAndEncodedToken) && CheckToken(signedAndEncodedToken))
                {
                    //var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
                    _GlobalContext.Token.Add(new Token()
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

                if (TokenExists(signedAndEncodedToken))
                {
                    _GlobalContext.Token.Remove(
                        _GlobalContext.Token.First(token => token.Token1.Equals(signedAndEncodedToken)));
                    changes = true;
                }

                if (changes)
                    _GlobalContext.SaveChanges();

                return new {Message = signedAndEncodedToken};
            }
            /*
            catch (DbEntityValidationException e) {
                String final = "";
                foreach (var eve in e.EntityValidationErrors){
                    final += String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        final += String.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return final;
            } */
            catch (Exception e)
            {
                return new { Message = $"An error ocurred during the validation process {e}" };
            }
        }
    }
}