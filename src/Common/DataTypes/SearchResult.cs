using System;

namespace Sympli.SEO.Common.DataTypes
{
    public class SearchResult
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Url { get; set; }

        public string[] Keywords { get; set; }

        public string KeywordsJoined { get; set; }

        public int[] Results { get; set; }

        public string ResultsJoined { get; set; }
    }
}
