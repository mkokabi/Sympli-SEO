using Microsoft.EntityFrameworkCore;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SearchResultsRepo: ISearchResultsRepo
    {
        private readonly SearchResultsContext context;

        public SearchResultsRepo(SearchResultsContext context)
        {
            this.context = context;
        }

        public async Task Add(SearchResult searchResult)
        {
            searchResult.KeywordsJoined = string.Join(",", searchResult.Keywords);
            searchResult.ResultsJoined = string.Join(",", searchResult.Results.Select(r => r.ToString()));
            context.SearchResults.Add(searchResult);
            await context.SaveChangesAsync();
        }

        public async Task<SearchResult> GetLatestSimilar(SearchResult searchResult)
        {
            var keywordsJoined = string.Join(",", searchResult.Keywords);
            return await context
                .SearchResults.Where(sr => sr.KeywordsJoined == keywordsJoined && sr.Url == searchResult.Url)
                .OrderByDescending(sr => sr.Date)
                .SingleOrDefaultAsync();
        }
    }
}
