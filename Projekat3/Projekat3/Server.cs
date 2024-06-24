using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class Server
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly NewsService service = new NewsService();

        public Server()
        {
            listener.Prefixes.Add("http://localhost:8000/");
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Server listening on port 8000");
            while (true)
            {
                var context = await listener.GetContextAsync();
                Task.Run(() => HandleRequestAsync(context));
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            if (context.Request.Url?.AbsolutePath == "/favicon.ico")
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
                return;
            }

            Console.WriteLine("Request: " + context.Request.HttpMethod + " " + context.Request.Url);
            var keyword = context.Request.QueryString["keyword"];
            var category = context.Request.QueryString["category"];


            if (string.IsNullOrEmpty(keyword))
            {
                Console.WriteLine("Bad request. Keyword parameter is missing.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Keyword parameter is missing. Request should look like this: 'http://localhost:8000/?keyword=KEY_WORD&category=VALID_CATEGORY'");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;
            }
            string[] categories = { "business", "entertainment", "general", "health", "science", "sports", "technology" };
            bool goodCateg = false;
            if (string.IsNullOrEmpty(category))
            {
                Console.WriteLine("Bad request. Category parameter is missing.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Category parameter is missing. Request should look like this: 'http://localhost:8000/?keyword=KEY_WORD&category=VALID_CATEGORY'");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;

            }
            foreach (string categ in categories)
            {
                if (categ == category)
                {
                    goodCateg = true;
                }
            }
            if (!goodCateg)
            {
                Console.WriteLine("Bad request. Category input is not valid.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Category parameter is not recognized. Valid categories are: \"business\", \"entertainment\", \"general\", \"health\", \"science\", \"sports\", \"technology\"");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;
            }

            var observer = new NewsObserver("ArticleObserver");
            var subject = new NewsStream();


            var articles = await service.FetchArticles(keyword, category);
            subject
                .Subscribe(observer);
            Console.WriteLine($"Sending request to NEWS API");
            Console.WriteLine("\nNews articles:\n");
            await subject.GetArticlesAsync(keyword, category);
            if (!subject.HasArticles)
            {
                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes("There are no articles with such keyword!");
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                var topics = TopicModelling.GetTopics(articles.Select(a => a.Title));
                var json = new JObject
                {
                    ["articles"] = new JArray(articles.Select(a => new JObject
                    {
                        ["title"] = a.Title,
                        ["source"] = a.Source,
                        ["topics"] = new JArray(a.Topics.Select(t => new JValue(t))),
                    })),
                    ["all articles topics:"] = new JArray(topics)
                };
                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                Console.WriteLine("Request successfully processed and resonse sent to client!");
            }
            
        }
    }
}
