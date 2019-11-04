using Microsoft.EntityFrameworkCore;
using Sympli.SEO.Common;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using System;
using System.Linq;
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

        public async Task Add(SearchParams searchParams, SearchResult searchResult)
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
                    SearchEngineId = searchParams.SearchEngineIndex
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

        public async Task<SearchResult> GetLatestSimilar(SearchParams searchParams)
        {
            var keywordsJoined = string.Join(",", searchParams.Keywords);
            var foundSearch = await context
                .Searches
                .AsNoTracking()
                .Where(s => s.Keywords == keywordsJoined && s.Url == searchParams.Url && s.SearchEngineId == searchParams.SearchEngineIndex)
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
                Results = foundSearchResult
                    .Result.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s)).ToArray()
            };
        }

        public async Task<PagedResponse<SearchResult>> GetResults(int startIndex, int pageSize)
        {
            var searchResults = (await this.context
                .SearchResults
                .Include(sr => sr.Search)
                .AsNoTracking()
                .Skip(startIndex)
                .Take(pageSize)
                .ToListAsync())
                .Select(s => new SearchResult
                {
                    Date = s.DateTime,
                    Keywords = s.Search.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries),
                    SearchEngineIndex = s.Search.SearchEngineId,
                    Url = s.Search.Url,
                    Results = s.Result.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.Parse(s)).ToArray()
                });
            var totalCounts = await this.context.SearchResults.CountAsync();
            return new PagedResponse<SearchResult>
            {
                Length = totalCounts,
                Results = searchResults
            };
        }
    }
}
