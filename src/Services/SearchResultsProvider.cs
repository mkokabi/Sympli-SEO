using Sympli.SEO.Common.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sympli.SEO.Services
{
    public class SearchResultsProvider : ISearchResultsProvider
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SearchResultsProvider(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string UrlInResultPattern => @"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>";

        public async Task<string> SearchForKeywords(string[] keywords)
        {
            var httpClient = this.httpClientFactory.CreateClient();
            var httpResponse = await httpClient.GetAsync($"https://google.com/search?q={string.Join("+", keywords)}");
            var responseStr = await httpResponse.Content.ReadAsStringAsync();
            return responseStr;
        }
    }
}
