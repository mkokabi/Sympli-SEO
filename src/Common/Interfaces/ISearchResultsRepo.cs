using Sympli.SEO.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchResultsRepo
    {
        Task Add(SearchParams searchParams, SearchResult searchResult);
        Task AddResult(Guid searchId, SearchResult searchResult);
        Task<SearchResult> GetLatestSimilar(SearchParams searchParams);
        Task<PagedResponse<SearchResult>> GetResults(int startIndex, int pageSize);
    }
}
