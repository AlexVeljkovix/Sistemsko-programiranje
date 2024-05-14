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

        public string GetArtworks(string query)
        {
            try
            {
                string url = $"{baseUrl}?q={query}";
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    return responseBody;
                }
                else
                {
                    return "Error while trying to return artwork data!";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
