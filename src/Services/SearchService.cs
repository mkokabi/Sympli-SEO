using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sympli.SEO.Services
{
    public class SearchService : ISearchService
    {
        public IEnumerable<SearchResult> GetResults()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new SearchResult
            {
                Date = DateTime.Now.AddDays(index),
                Keywords = new[] { "Abc", "Def" },
                Url = "www.abc.com",
                Results = new int[] { 1, 3, 5 }
            })
            .ToArray();
        }

        public SearchResult Search(SearchParams searchParams)
        {
            return new SearchResult
            {
                Date = DateTime.Now,
                Keywords = searchParams.Keywords,
                Url = searchParams.Url,
                Results = new int[] { 4, 5, 6 }
            };
        }
    }
}
