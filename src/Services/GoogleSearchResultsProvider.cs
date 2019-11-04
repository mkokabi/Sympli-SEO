using Sympli.SEO.Common.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sympli.SEO.Services
{
    public class GoogleSearchResultsProvider : ISearchResultsProvider
    {
        private readonly IHttpClientFactory httpClientFactory;

        public GoogleSearchResultsProvider(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string UrlInResultPattern => @"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>";

        public string RemoveTralier(string withTrailer)
        {
            var trailerStart = withTrailer.IndexOf(" &#8250; ");
            if (trailerStart == -1)
            {
                return withTrailer;
            }
            var trailerEnd = withTrailer.IndexOf("<", trailerStart);
            return withTrailer.Substring(0, trailerStart) + withTrailer.Substring(trailerEnd);
        }

        public async Task<string> SearchForKeywords(string[] keywords)
        {
            var httpClient = this.httpClientFactory.CreateClient();
            var httpResponse = await httpClient.GetAsync($"https://google.com/search?q={string.Join("+", keywords)}&num=100");
            var responseStr = await httpResponse.Content.ReadAsStringAsync();
            return responseStr;
        }
    }
}
