using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Projekat3
{
    public class NewsStream : IObservable<News>
    {
        private readonly Subject<News> subject = new Subject<News>();
        private readonly NewsService service = new NewsService();
        public bool HasArticles { get; private set; }

        public async Task GetArticlesAsync(string query, string category)
        {
            try
            {
                var articles = await service.FetchArticles(query, category);

                HasArticles = articles != null && articles.Any();

                foreach (var article in articles)
                {
                    subject.OnNext(article);
                }
                subject.OnCompleted();
            }
            catch (Exception ex)
            {
                subject.OnError(ex);
            }
        }

        public IDisposable Subscribe(IObserver<News> observer)
        {
            return subject
                .SubscribeOn(TaskPoolScheduler.Default)
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(observer);
        }
    }
}
