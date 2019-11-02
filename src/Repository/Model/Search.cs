using System;

namespace Repository.Model
{
    public class Search
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Keywords { get; set; }
        public DateTime DateTime { get; set; }
        public int SearchEngineId { get; set; }
    }
}
