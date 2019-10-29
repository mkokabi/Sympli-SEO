using Sympli.SEO.Common.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> GetResults();
        Task<SearchResult> Search(SearchParams searchParams);
    }
}
