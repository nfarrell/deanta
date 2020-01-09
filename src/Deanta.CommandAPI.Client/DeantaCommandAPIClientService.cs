using Deanta.Models.Contracts;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deanta.CommandAPI.Client
{
    public class DeantaCommandApiClientService : IDeantaCommandApiClientService
    {
        private readonly IHttpClientFactory _clientFactory;

        public DeantaCommandApiClientService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task CreateTodo(TodoDto newTask, Guid userId)
        {
            var client = _clientFactory.CreateClient("TodoCommandAPI");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Todo/todos");
            var jsonBody = JsonConvert.SerializeObject(newTask);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            request.Headers.Add("UserId", userId.ToString());
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                //do something
                return;
            }
        }

        public async Task UpdateTodo(Guid id, TodoDto updatedTodo, Guid userId)
        {
            var client = _clientFactory.CreateClient("TodoCommandAPI");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Todo/todos/{id}");
            var jsonBody = JsonConvert.SerializeObject(updatedTodo);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            request.Headers.Add("UserId", userId.ToString());
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                //do something
                return;
            }
        }
    }
}
