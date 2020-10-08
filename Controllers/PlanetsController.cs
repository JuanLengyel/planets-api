using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace planets_api_v2.Controllers
{
    [Route("planets")]
    public class PlanetsController : Controller
    {

        class Auth
        {
            public String email { get; set; }
            public String passphrase { get; set; }
        }

        class UserToken
        {
            public String hi { get; set; }
            public String dont_tell_anyone_this_token { get; set; }
        }

        [HttpGet]
        public IEnumerable<Planet> Get()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://code-challenge.proximitycr.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            // Make token request
            Auth auth = new Auth
            {
                email = "juan.lengyel@hotmail.com",
                passphrase = "juan_zuniga-net"
            };

            UserToken token = getRequestToken(client, auth).Result;

            client.DefaultRequestHeaders.Add("Authorization", token.dont_tell_anyone_this_token);

            return getPlanets(client).Result;
        }

        static async Task<UserToken> getRequestToken(HttpClient client, Auth auth) {
            var json = JsonConvert.SerializeObject(auth);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("gimme-the-token", data);

            return JsonConvert.DeserializeObject<UserToken>(response.Content.ReadAsStringAsync().Result);
        }

        static async Task<List<Planet>> getPlanets(HttpClient client) {
            var response = await client.GetAsync("planets");

            return JsonConvert.DeserializeObject<List<Planet>>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
