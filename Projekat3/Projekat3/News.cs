using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class News
    {
        public string Title { get; set; }
        public JObject Source { get; set; }
        public List<string> Topics { get; set; }
    }
}
