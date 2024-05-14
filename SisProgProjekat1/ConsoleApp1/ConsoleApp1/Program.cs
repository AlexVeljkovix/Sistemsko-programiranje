using System;

namespace Projekat
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = $"https://api.artic.edu/api/v1/artworks/search";
            int cacheCapacity = 10;
            var server = new Server(baseUrl, cacheCapacity);
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            server.Stop();
        }
    }
}