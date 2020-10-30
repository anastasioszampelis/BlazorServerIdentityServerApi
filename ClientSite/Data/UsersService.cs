using ClientShared.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientSite.Data
{
    public interface IUsersService
    {
        public Task<Tuple<bool, List<UserDto>, string>> GetUsers();
    }
    public class UsersService : IUsersService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Tuple<bool, List<UserDto>, string>> GetUsers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
            "Users");

            var client = _clientFactory.CreateClient("ApiClient");

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            if (accessToken != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            }

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<List<UserDto>>(responseString);
                return new Tuple<bool, List<UserDto>, string>(true, userResponse, string.Empty);
            }
            else
            {
                return new Tuple<bool, List<UserDto>, string>(false, null, response.ReasonPhrase);
            }
        }
    }
}
