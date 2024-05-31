using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Projekat
{
    public class AicApiClient
    {
        private string baseUrl;
        private HttpClient client;

        public AicApiClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            client = new HttpClient();
        }

        //promenjeno na Task
        public async Task<string> GetArtworks(string query)
        {
            try
            {
                string url = $"{baseUrl}?q={query}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Error: Failed to retrieve artworks.");
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
