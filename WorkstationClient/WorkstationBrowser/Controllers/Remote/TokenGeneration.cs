using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WorkstationBrowser.Controllers.Remote {
    public static class TokenGeneration {
        public static Task<String> FetchToken(String Username, String Password) {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Workstation Probing Agent");
            client.DefaultRequestHeaders.Add("Username", Username);
            client.DefaultRequestHeaders.Add("Password", Password);
            
            var stringTask = client.GetStringAsync("http://localhost:15572/api/Tokens/Generate");

            return stringTask;// (serializer.ReadObject(await streamTask) as MessageContract);

        }
    }
}