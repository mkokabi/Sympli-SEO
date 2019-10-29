using FluentAssertions;
using NSubstitute;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using Sympli.SEO.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ServiceUnitTests
{
    public class SearchServiceTestsWithRealData
    {
        private ISearchResultsProvider searchResultProvider;

        public SearchServiceTestsWithRealData()
        {
            searchResultProvider = Substitute.For<ISearchResultsProvider>();
        }

        [Fact]
        public void Google_search_for_test_keywords()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(File.ReadAllText("SearchResponse_01.html"));
            searchResultProvider.UrlInResultPattern.Returns(@"<div class=""BNeawe UPmit AP7Wnd"">{url}</div>");

            var searchService = new SearchService(this.searchResultProvider);
            var result = searchService.Search(new SearchParams { Url = "https://keywordtool.io", Keywords = new string[] { "test", "keywords" } });
            result.Results.Should().HaveCount(1);
            result.Results[0].Should().Be(0);
        }
    }
}
