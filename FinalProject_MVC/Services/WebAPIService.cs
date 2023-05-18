using FinalProject_MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace FinalProject_MVC.Services
{
    public interface IWebAPIService
    {
        Task<JObject?> AuthenticateAsync(string email, string password);
        Task<JObject?> RegisterAsync(RegisterModel model);
    }
    public class WebAPIService : IWebAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public WebAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JObject?> AuthenticateAsync(string email, string password)
        {
            //content are JSON format
            var content = new StringContent(JsonConvert.SerializeObject(new { email, password }), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("WebApi");

            var response = await httpClient.PostAsync("/api/auth/login", content);

                //read token from response, format {"name": "", "token": ""}
                var responseContent = await response.Content.ReadAsStringAsync();

                //convert to JObject
                var responseJson = JObject.Parse(responseContent);

                return responseJson;
        }

        public async Task<JObject?> RegisterAsync(RegisterModel model)
        {
            //content are JSON format
            var content = new StringContent(JsonConvert.SerializeObject(new {
                Email = model.Email,
                Password = model.Password,
                Hoten = model.Name,
                Sdt = model.Phone,
                Diachi = model.Address
            }), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("WebApi");

            var response = await httpClient.PostAsync("/api/auth/register", content);

            //read token from response, format {"name": "", "token": ""}
            var responseContent = await response.Content.ReadAsStringAsync();

            //convert to JObject
            var responseJson = JObject.Parse(responseContent);

            return responseJson;
        }
    }
}
