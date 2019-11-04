using Sympli.SEO.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sympli.SEO.Services
{
    public class BingSearchResultProvider : ISearchResultsProvider
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BingSearchResultProvider(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public string UrlInResultPattern => "<cite>{url}</cite>";

        public string RemoveTralier(string withTrailer) => withTrailer.Replace("<strong>", "").Replace("</strong>", "");


        public async Task<string> SearchForKeywords(string[] keywords)
        {
            var httpClient = this.httpClientFactory.CreateClient();
            var httpResponse = await httpClient.GetAsync($"https://bing.com/search?q={string.Join("+", keywords)}&count=100");
            var responseStr = await httpResponse.Content.ReadAsStringAsync();
            return responseStr;
        }
    }
}
