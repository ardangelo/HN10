using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace HN10.API {
    public enum ItemType { Job, Story, Comment, Poll, Pollopt };
    public class HNItem {
        public int id { get; set; }
        public bool deleted { get; set; }
        public ItemType type { get; set; }
        public string by { get; set; }
        public int time { get; set; }
        public string text { get; set; }
        public bool dead { get; set; }
        public int parent { get; set; }
        public List<int> kids { get; set; }
        public string url { get; set; }
        public int score { get; set; }
        public string title { get; set; }
        public List<string> parts { get; set; }
        public int decendants { get; set; }

        public HNItem() { }

        async public static Task<HNItem> fromID(int id, HttpClient client) {
            var uri = new Uri("https://hacker-news.firebaseio.com/v0/item/" + id + ".json");
            System.Diagnostics.Debug.WriteLine("Requesting " + uri.ToString());
            var raw = await client.GetStringAsync(uri);
            var root = JsonConvert.DeserializeObject<HNItem>(raw);

            return root;
        }

        async public static Task<HNItem> fromID(int id) {
            var client = new HttpClient();

            var raw = await client.GetStringAsync(new Uri("https://hacker-news.firebaseio.com/v0/item/" + id + ".json"));
            var root = JsonConvert.DeserializeObject<HNItem>(raw);

            return root;
        }
    }
}
