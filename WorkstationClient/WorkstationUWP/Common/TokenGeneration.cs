using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkstationUWP.Common {
    public static class TokenGeneration {
        public static Task<String> FetchToken(String Username, String Password) {
            RequestHandler.Client.DefaultRequestHeaders.Add("User-Agent", "Workstation Probing Agent");
            RequestHandler.Client.DefaultRequestHeaders.Add("Username", Username);
            RequestHandler.Client.DefaultRequestHeaders.Add("Password", Password);
            
            var stringTask = RequestHandler.Client.GetStringAsync("http://localhost:15572/api/Tokens/Generate");

            return stringTask;// (serializer.ReadObject(await streamTask) as MessageContract);

        }
    }
}