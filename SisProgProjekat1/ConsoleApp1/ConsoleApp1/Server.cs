using System;
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

        public void Start()
        {
            Console.WriteLine("Web server started and listening!");
            try
            {
                listener.Start();
                while (listener.IsListening)
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessRequest(object? state)
        {
            try
            {
                if (state == null)
                {
                    return;
                }
                Console.WriteLine("Processing request!");
                var context = (HttpListenerContext)state;
                if (context.Request.HttpMethod != "GET")
                {
                    MakeResponse(context, "Please use GET method!", true);
                    return;
                }
                if (context.Request.Url?.Query == "")
                {
                    MakeResponse(context, "Empty query!", true);
                    return;
                }

                var param = (context.Request.Url?.Query.Remove(0, 1).Split("="));
                if (param == null)
                {
                    MakeResponse(context, "Null query!", true);
                    throw new Exception("Null query!");
                }

                var query = "";
                if (param[0] == "q")
                {
                    query = param[1];
                }
                else
                {
                    MakeResponse(context, "Please use full text search!", true);
                    return;
                }

                byte[] cachedResponse = cache.Get(query);
                JObject data;

                if (cachedResponse != null)
                {
                    Console.WriteLine("Cache hit!");
                    data = JObject.Parse(Encoding.UTF8.GetString(cachedResponse));
                }
                else
                {
                    try
                    {
                        string artworks = client.GetArtworks(query);
                        data = JObject.Parse(artworks);
                        cache.Set(query, Encoding.UTF8.GetBytes(artworks));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with Art Institute of Chicago api: {ex.Message}");
                        MakeResponse(context, "Internal server error.", true);
                        return;
                    }
                }

                Console.WriteLine("Artwork data:");
                Console.WriteLine(data);
                MakeResponse(context, data.ToString(), false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        public void Stop()
        {
            Console.WriteLine("Server is not listening anymore.");
            if (!disposed) return;
            else
            {
                listener.Stop();
                listener.Close();
                disposed = true;
            }
        }

        private void MakeResponse(HttpListenerContext context, string responseContent, bool badRequest)
        {
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(responseContent);
            response.ContentLength64 = buffer.Length;

            try
            {
                using (var outputString = response.OutputStream)
                {
                    outputString.Write(buffer, 0, buffer.Length);
                }

                response.ContentType = "text/html";
                if (badRequest)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "Bad Request";
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusDescription = "OK";
                }
                response.ContentEncoding = Encoding.UTF8;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending response: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }
    }
}
