using System.Threading.Tasks;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchResultsProvider
    {
        Task<string> SearchForKeywords(string[] keywords);
        string UrlInResultPattern { get; }
        string RemoveTralier(string withTrailer);
    }
}
