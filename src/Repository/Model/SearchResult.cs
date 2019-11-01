using System;

namespace Repository.Model
{
    public class SearchResult
    {
        public Guid SearchResultId { get; set; }
        public Guid SearchId { get; set; }
        public DateTime DateTime { get; set; }
        public string Result { get; set; }
    }
}
