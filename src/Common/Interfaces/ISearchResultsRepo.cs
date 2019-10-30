using Sympli.SEO.Common.DataTypes;
using System.Threading.Tasks;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchResultsRepo
    {
        Task Add(SearchResult searchResult);

        Task<SearchResult> GetLatestSimilar(SearchResult searchResult);
    }
}
