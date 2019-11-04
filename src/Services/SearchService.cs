using Microsoft.Extensions.Logging;
using Sympli.SEO.Common;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sympli.SEO.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchResultsProvider[] searchResultsProviders;
        private readonly ISearchResultsRepo searchResultsRepo;

        public ILoggerFactory LoggerFactory { get; }

        public SearchService(
            IEnumerable<ISearchResultsProvider> searchResultsProviders,
            ISearchResultsRepo searchResultsRepo,
            ILoggerFactory loggerFactory)
        {
            this.searchResultsProviders = searchResultsProviders.ToArray();
            this.searchResultsRepo = searchResultsRepo;
            LoggerFactory = loggerFactory;
        }

        public async Task<PagedResponse<SearchResult>> GetResults(int startIndex, int pageSize)
        {
            return await this.searchResultsRepo.GetResults(startIndex, pageSize);
        }

        public async Task<SearchResult> Search(SearchParams searchParams, int searchEngineIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(searchParams.Url))
            {
                throw new ArgumentOutOfRangeException("URL not provided");
            }
            if (!searchParams.Keywords.Any())
            {
                throw new ArgumentOutOfRangeException("There should be at least one keyword");
            }
            if (searchEngineIndex < 0 || searchEngineIndex >= this.searchResultsProviders.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid searchEngineIndex");
            }

            var logger = this.LoggerFactory.CreateLogger<SearchService>();
            logger.LogDebug("Search params {searchParams}", searchParams);

            var similarSearchResult = await this.searchResultsRepo.GetLatestSimilar(
                new SearchParams { Keywords = searchParams.Keywords, Url = searchParams.Url, SearchEngineIndex = searchEngineIndex});
            if (similarSearchResult != null && similarSearchResult.Date.AddHours(1) > DateTime.Now)
            {
                return similarSearchResult;
            }
            var seResponse = await this.searchResultsProviders[searchEngineIndex].SearchForKeywords(searchParams.Keywords);
            var allOccurences = BreakResponse(seResponse, this.searchResultsProviders[searchEngineIndex].UrlInResultPattern);
            var occurencesLoc = new List<int>(allOccurences.Length);
            var patternParts = this
                .searchResultsProviders[searchEngineIndex]
                .UrlInResultPattern.Split(new[] { "{url}" }, StringSplitOptions.RemoveEmptyEntries);
            var beforePart = patternParts[0];
            var afterPart = patternParts[1];
            for (int i = 0; i < allOccurences.Count(); i++)
            {
                //remove trailer
                var occurence = this.searchResultsProviders[searchEngineIndex].RemoveTralier(allOccurences[i]);
                occurence = occurence.Replace(beforePart, "").Replace(afterPart, "");
                if (SameDomain(occurence, searchParams.Url))
                {
                    occurencesLoc.Add(i);
                }
            }
            var result = new SearchResult
            {
                Date = DateTime.Now,
                Keywords = searchParams.Keywords,
                Url = searchParams.Url,
                Results = occurencesLoc.ToArray()
            };
            await this.searchResultsRepo.Add(searchParams, result);
            return result;
        }

        private bool SameDomain(string occurence, string searchUrl)
        {
            if (occurence.Equals(searchUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            if (Uri.TryCreate(occurence, UriKind.Absolute, out Uri occurenceUri))
            {
                if (Uri.TryCreate(searchUrl, UriKind.Absolute, out Uri searchUrlUri))
                {
                    return occurenceUri.DnsSafeHost.Equals(searchUrlUri.DnsSafeHost, StringComparison.InvariantCultureIgnoreCase);
                }
                return occurenceUri.DnsSafeHost.Equals(searchUrl, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private string[] BreakResponse(string seResponse, string urlInResultPattern)
        {
            // TODO: Complete the regex pattern replacement
            var pattern = urlInResultPattern.Replace("{url}", ".*?").Replace("/", @"\/");
            var regex = new Regex(pattern);
            var regexMatches = regex.Matches(seResponse);
            var results = new List<string>(regexMatches.Count);
            var enumerator = regexMatches.GetEnumerator();
            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.ToString());
            }
            return results.ToArray();
        }
    }
}
