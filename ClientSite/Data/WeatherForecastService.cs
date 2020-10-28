using ClientShared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ClientSite.Data
{
    public class WeatherForecastService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeatherForecastService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
            "WeatherForecast");

            var client = _clientFactory.CreateClient("ApiClient");
            
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (accessToken != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            }

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                var forecast = JsonConvert.DeserializeObject<WeatherForecast[]>(responseStream);
                return forecast;
            }
            else
            {
                return null;   
            }

        }
    }
}
