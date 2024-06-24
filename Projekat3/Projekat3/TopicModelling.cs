using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class TopicModelling
    {
        public static List<string> GetTopics(IEnumerable<string> titles)
        {
            var topics = new List<string>();

            foreach (var title in titles)
            {
                var words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    topics.Add(word.ToLower());
                }
            }

            return topics.Distinct().ToList();
        }
    }
}
