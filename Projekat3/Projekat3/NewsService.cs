using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    internal class NewsService
    {
        private readonly string API_KEY = "e43bf19ea4724a79b373071e1e8b0544";

        public async Task<IEnumerable<News>> FetchArticles(string query, string category)
        {
            HttpClient client = new HttpClient();
            var url = $"https://newsapi.org/v2/top-headlines?q={Uri.EscapeDataString(query)}&category={Uri.EscapeDataString(category)}&apiKey={API_KEY}";


            client.DefaultRequestHeaders.Add("User-Agent", "Projekat3/1.0");

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API response error: {errorContent}");
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);
                var articlesJson = jsonResponse["articles"];

                if (articlesJson == null)
                {
                    return Enumerable.Empty<News>();
                }
                if(articlesJson.Count() == 0)
                {
                    Console.WriteLine("There are no articles with that keyword!");
                    return Enumerable.Empty<News>();
                }
                var articles = articlesJson.Select(article => new News
                {

                    Title = (string)article["title"],
                    Source = (JObject)article["source"],
                    Topics = TopicModelling.GetTopics(new List<string> { (string)article["title"] })

                });
                return articles;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"API HTTP Request Error: {httpEx.Message}");
                return Enumerable.Empty<News>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal API Error: {ex.Message}");
                return Enumerable.Empty<News>();
            }
        }
    }
}
