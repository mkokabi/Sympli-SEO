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
            var searchId = Guid.NewGuid();
            var searchDate = DateTime.Now;
            context.Searches.Add(
                new Model.Search
                {
                    Id = searchId,
                    Keywords = string.Join(",", searchResult.Keywords),
                    DateTime = searchDate,
                    Url = searchResult.Url,
                    SearchEngineId = 1
                });
            context.SearchResults.Add(
                new Model.SearchResult { 
                    SearchResultId = Guid.NewGuid(),
                    Result = string.Join(",", searchResult.Results.Select(r => r.ToString())),
                    DateTime = searchDate,
                    SearchId = searchId
                });
            await context.SaveChangesAsync();
        }

        public async Task AddResult(Guid searchId, SearchResult searchResult)
        {
            context.SearchResults.Add(
                new Model.SearchResult
                {
                    SearchResultId = Guid.NewGuid(),
                    Result = string.Join(",", searchResult.Results.Select(r => r.ToString())),
                    DateTime = DateTime.Now,
                    SearchId = searchId
                });
            await context.SaveChangesAsync();
        }

        public async Task<SearchResult> GetLatestSimilar(SearchResult searchResult)
        {
            var keywordsJoined = string.Join(",", searchResult.Keywords);
            var foundSearch = await context
                .Searches
                .AsNoTracking()
                .Where(s => s.Keywords == keywordsJoined && s.Url == searchResult.Url)
                .OrderByDescending(sr => sr.DateTime)
                .SingleOrDefaultAsync();
            if (foundSearch == null)
            {
                return null;
            }
            var foundSearchResult = await context
                .SearchResults
                .AsNoTracking()
                .Where(sr => sr.SearchId == foundSearch.Id)
                .OrderByDescending(sr => sr.DateTime)
                .SingleOrDefaultAsync();

            return new SearchResult
            {
                Date = foundSearchResult.DateTime,
                Keywords = foundSearch.Keywords.Split(","),
                Url = foundSearch.Url,
                Results = foundSearchResult.Result.Split(",").Select(s => int.Parse(s)).ToArray()
            };
        }
    }
}
