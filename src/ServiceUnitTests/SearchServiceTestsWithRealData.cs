using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using Sympli.SEO.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ServiceUnitTests
{
    public class SearchServiceTestsWithRealData
    {
        private ISearchResultsProvider searchResultProvider;

        private ISearchResultsRepo searchResultsRepo;

        private ILoggerFactory loggerFactory;

        public SearchServiceTestsWithRealData()
        {
            searchResultProvider = Substitute.For<ISearchResultsProvider>();

            searchResultsRepo = Substitute.For<ISearchResultsRepo>();

            loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger("").Returns(NullLogger.Instance);
        }

        [Fact]
        public async Task Google_search_for_test_keywords()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("SearchResponse_01.html"));
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => callInfo.Args()[0]);
            searchResultProvider.UrlInResultPattern.Returns(@"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>");

            var searchService = new SearchService(new[] { this.searchResultProvider }, this.searchResultsRepo, this.loggerFactory);
            var result = await searchService.Search(new SearchParams { Url = "https://keywordtool.io", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(0);
        }

        private string RemoveTralier(string withTrailer)
        {
            var trailerStart = withTrailer.IndexOf(" &#8250; ");
            if (trailerStart == -1)
            {
                return withTrailer;
            }
            var trailerEnd = withTrailer.IndexOf("<", trailerStart);
            return withTrailer.Substring(0, trailerStart) + withTrailer.Substring(trailerEnd);
        }

        [Fact]
        public async Task Google_search_for_test_keywords_while_result_has_trailer()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("SearchResponse_01.html"));
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => RemoveTralier(callInfo.Args()[0].ToString()));
            searchResultProvider.UrlInResultPattern.Returns(@"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>");

            var searchService = new SearchService(new[] { this.searchResultProvider }, this.searchResultsRepo, this.loggerFactory);
            var result = await searchService.Search(new SearchParams { Url = "https://moz.com", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(3);
        }

        [Fact]
        public async Task Google_search_for_test_keywords_while_result_has_trailer_without_scheme()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("SearchResponse_01.html"));
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => RemoveTralier(callInfo.Args()[0].ToString()));
            searchResultProvider.UrlInResultPattern.Returns(@"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>");

            var searchService = new SearchService(new[] { this.searchResultProvider }, this.searchResultsRepo, this.loggerFactory);
            var result = await searchService.Search(new SearchParams { Url = "moz.com", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(3);
        }

        [Fact]
        public async Task Google_search_for_test_keywords_while_result_has_trailer_with_different_scheme()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("SearchResponse_01.html"));
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => RemoveTralier(callInfo.Args()[0].ToString()));
            searchResultProvider.UrlInResultPattern.Returns(@"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>");

            var searchService = new SearchService(new[] { this.searchResultProvider }, this.searchResultsRepo, this.loggerFactory);
            var result = await searchService.Search(new SearchParams { Url = "http://moz.com", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(3);
        }

        private string BingTrailerRemover(string input)
        {
            return input.Replace("<strong>", "").Replace("</strong>", "");
        }

        [Fact]
        public async Task Bing_search_for_test_keywords_while_result_has_trailer()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("BingSearchResponse_01.html"));
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => BingTrailerRemover(callInfo.Args()[0].ToString()));
            searchResultProvider.UrlInResultPattern.Returns(@"<cite>{url}</cite>");

            var searchService = new SearchService(new[] { this.searchResultProvider }, this.searchResultsRepo, this.loggerFactory);
            var result = await searchService.Search(new SearchParams { Url = "www.sporcle.com", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(31);
        }
    }
}
