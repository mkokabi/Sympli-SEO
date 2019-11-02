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
        private readonly ISearchResultsProvider searchResultsProvider;
        private readonly ISearchResultsRepo searchResultsRepo;

        public SearchService(
            ISearchResultsProvider searchResultsProvider,
            ISearchResultsRepo searchResultsRepo)
        {
            this.searchResultsProvider = searchResultsProvider;
            this.searchResultsRepo = searchResultsRepo;
        }

        public async Task<IEnumerable<SearchResult>> GetResults()
        {
            return await this.searchResultsRepo.GetResults(1, 10);
            //var rng = new Random();
            //return await Task.FromResult(Enumerable.Range(1, 5).Select(index => new SearchResult
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    Keywords = new[] { "Abc", "Def" },
            //    Url = "www.abc.com",
            //    Results = new int[] { 1, 3, 5 }
            //})
            //.ToArray());
        }

        public async Task<SearchResult> Search(SearchParams searchParams)
        {
            if (string.IsNullOrWhiteSpace(searchParams.Url))
            {
                throw new ArgumentOutOfRangeException("URL not provided");
            }
            if (!searchParams.Keywords.Any())
            {
                throw new ArgumentOutOfRangeException("There should be at least one keyword");
            }

            var similarSearchResult = await this.searchResultsRepo.GetLatestSimilar(new SearchResult { Keywords = searchParams.Keywords, Url = searchParams.Url});
            if (similarSearchResult != null && similarSearchResult.Date.AddHours(1) > DateTime.Now)
            {
                return similarSearchResult;
            }
            var seResponse = await this.searchResultsProvider.SearchForKeywords(searchParams.Keywords);
            var allOccurences = BreakResponse(seResponse, this.searchResultsProvider.UrlInResultPattern);
            var occurencesLoc = new List<int>(allOccurences.Length);
            var urlInPattern = this.searchResultsProvider.UrlInResultPattern.Replace("{url}", searchParams.Url);
            for (int i = 0; i < allOccurences.Count(); i++)
            {
                //remove trailer
                var occurence = this.searchResultsProvider.RemoveTralier(allOccurences[i]);

                if (occurence.Equals(urlInPattern, StringComparison.InvariantCultureIgnoreCase))
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
            await this.searchResultsRepo.Add(result);
            return result;
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
