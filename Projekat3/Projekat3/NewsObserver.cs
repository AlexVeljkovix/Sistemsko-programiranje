using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class NewsObserver : IObserver<News>
    {
        private readonly string name;

        public NewsObserver(string name)
        {
            this.name = name;
        }

        public void OnNext(News article)
        {
            Console.WriteLine($"Title: {article.Title}\n" +
                              $"Source: {article.Source}\n" +
                              $"Topics: \n{ string.Join(", \n", article.Topics)}");
        }

        public void OnError(Exception e)
        {
            Console.WriteLine($"Error: {e.Message}\n");
        }

        public void OnCompleted()
        {
            Console.WriteLine("All articles processed!");
        }
    }
}
