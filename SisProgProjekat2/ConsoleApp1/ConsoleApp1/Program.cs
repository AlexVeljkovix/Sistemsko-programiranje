using System;

namespace Projekat
{
    class Program
    {
        //main promenjen u Task
        static async Task Main(string[] args)
        {
            string baseUrl = $"https://collectionapi.metmuseum.org/public/collection/v1/search";
            int cacheCapacity = 10;
            var server = new Server(baseUrl, cacheCapacity);
            try
            {
               await server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { server.Stop(); }

            Console.ReadLine();
        }
    }
}