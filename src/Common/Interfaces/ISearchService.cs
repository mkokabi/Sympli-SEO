using Sympli.SEO.Common.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchService
    {
        Task<PagedResponse<SearchResult>> GetResults(int startIndex, int pageSize);
        Task<SearchResult> Search(SearchParams searchParams);
    }
}
