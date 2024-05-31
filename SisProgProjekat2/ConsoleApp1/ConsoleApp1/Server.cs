using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Projekat;

namespace Projekat
{
    public class Server
    {
        private readonly AicApiClient client;
        private readonly Cache cache;
        private readonly HttpListener listener;
        private bool disposed = false;

        public Server(string baseUrl, int cacheCapacity, string address = "localhost", int port = 5050)
        {
            client = new AicApiClient(baseUrl);
            cache = new Cache(cacheCapacity);
            listener = new HttpListener();
            listener.Prefixes.Add($"http://{address}:{port}/");
        }

        //Promenjeno na Task
        public async Task Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Web server started and listening!");
                while (true)
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    
                    Task.Run(() => ProcessRequest(context));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ProcessRequest(HttpListenerContext context)
        {
            try
            {
                if (context == null)
                {
                    Console.WriteLine("Null context.");
                    return;
                }

                // Provera za favicon.ico zato sto browser automatski salje dva requesta, jedan validan, a onda odmah za njim jedan prazan
                if (context.Request.Url?.AbsolutePath == "/favicon.ico")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                    return;
                }


                Console.WriteLine("Processing request!");

                if (context.Request.HttpMethod != "GET")
                {
                    await MakeResponse(400, context, "Please use GET method!");
                    Console.WriteLine("Get method required.");
                    return;
                }
                if (context.Request.Url?.Query == "")
                {
                    await MakeResponse(400, context, "Empty query.");
                    Console.WriteLine("Empty query.");
                    return;
                }

                var param = (context.Request.Url?.Query.Remove(0, 1).Split("="));
                if (param == null)
                {
                    await MakeResponse(400, context, "Null query.");
                    Console.WriteLine("Null query.");
                    return;
                }

                var query = "";
                if (param[0] == "q")
                {
                    if (param.Length > 1 && param[1] != "")
                    {
                        query = param[1];
                    }
                    else
                    {
                        await MakeResponse(400, context, "No search parameter.");
                        Console.WriteLine("Empty query.");
                        return;
                    }
                }
                else
                {
                    await MakeResponse(400, context, "Please use full text search!");
                    Console.WriteLine("Invalid query parameter.");
                    return;
                }

                JObject data;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                byte[] cachedResponse = cache.Get(query);

                if (cachedResponse != null)
                {
                    sw.Stop();
                    data = JObject.Parse(Encoding.UTF8.GetString(cachedResponse));
                    Console.WriteLine("Cache hit!");
                    Console.WriteLine("Time elapsed when obtaining data from cache: " + sw.Elapsed);
                }
                else
                {
                    try
                    {
                        string artworks = await client.GetArtworks(query);
                        sw.Stop();
                        Console.WriteLine("Time elapsed when obtaining data from API: " + sw.Elapsed);
                        data = JObject.Parse(artworks);
                        cache.Set(query, Encoding.UTF8.GetBytes(artworks));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with API: {ex.Message}");
                        await MakeResponse(400, context, "Internal server error.");
                        return;
                    }
                }

                await MakeResponse(200, context, data.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

       

        public void Stop()
        {
            Console.WriteLine("Server is not listening anymore.");
            if (disposed) return;
            else
            {
                listener.Stop();
                listener.Close();
                cache.Clear();
                disposed = true;
            }
        }

        //promenjen u Task 

        private async Task MakeResponse(int responseCode, HttpListenerContext context, string text)
        {
            
            
                var response = context.Response;
                response.StatusCode = responseCode;

                string body;

                if (responseCode == 400)
                {
                    response.ContentType = "text/html";
                    body = $@"
            <html>
            <head>
                <title>Bad Request</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #fff;
                        padding: 20px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        border-radius: 8px;
                    }}
                    h1 {{
                        color: #d9534f;
                    }}
                    p {{
                        font-size: 18px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Bad Request</h1>
                    <p>{text}</p>
                </div>
            </body>
            </html>";
                }
                else if (responseCode == 200)
                {
                    response.ContentType = "application/json"; 
                    body = text;
                }
                else
                {
                    response.ContentType = "text/html";
                    body = $@"
            <html>
            <head>
                <title>Error</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #fff;
                        padding: 20px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        border-radius: 8px;
                    }}
                    h1 {{
                        color: #d9534f;
                    }}
                    p {{
                        font-size: 18px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Error</h1>
                    <p>{text}</p>
                </div>
            </body>
            </html>";
                }

                var buffer = Encoding.UTF8.GetBytes(body);
                response.ContentLength64 = buffer.Length;

                try
                {
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while sending response: {e.Message}");
                }
                finally
                {
                    response.Close();
                }
         

        }
    }
}
