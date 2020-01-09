using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Deanta.Models.Contracts;
using Deanta.Models.Paging;

namespace Deanta.QueryAPI.Client
{
    public class DeantaQueryApiClientService : IDeantaQueryApiClientService
    {
        private readonly IHttpClientFactory _clientFactory;

        public DeantaQueryApiClientService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<TodoDto> GetTodo(Guid id)
        {
            var client = _clientFactory.CreateClient("DeantaQueryAPI");
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Deanta/templates" + id);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var serializedResult = await response.Content.ReadAsStringAsync();
                var parsedResult = JObject.Parse(serializedResult);
                return parsedResult.ToObject<TodoDto>();
            }
            else
            {
                return null;
            }
        }

        public async Task<PagedList<TodoDto>> GetTodos(int? page = null,
            int? pageSize = 10, Dictionary<string, string> queryParameters = null)
        {
            var client = _clientFactory.CreateClient("DeantaQueryAPI");
            var queryParams = QueryHelpers.AddQueryString("/api/Deanta/todos", queryParameters);
            var request = new HttpRequestMessage(HttpMethod.Get, queryParams);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var serializedResult = await response.Content.ReadAsStringAsync();
                var parsedResult = JObject.Parse(serializedResult);
                return parsedResult.ToObject<PagedList<TodoDto>>();
            }
            else
            {
                return null;
            }
        }
    }
}
