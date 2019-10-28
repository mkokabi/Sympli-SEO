using FluentAssertions;
using NSubstitute;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using Sympli.SEO.Services;
using System;
using Xunit;

namespace ServiceUnitTests
{
    public class SearchServiceTests
    {
        private ISearchResultsProvider searchResultProvider;

        public SearchServiceTests()
        {
            searchResultProvider = Substitute.For<ISearchResultsProvider>();
        }

        [Fact]
        public void InvalidParameters_should_throw_exception_on_empty_url()
        {
            var searchService = new SearchService(this.searchResultProvider);
            Action searchAct = () =>
            searchService.Search(new SearchParams { Url = "", Keywords = new string[] { "a" } });
            searchAct.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void InvalidParameters_should_throw_exception_on_no_keywords()
        {
            var searchService = new SearchService(this.searchResultProvider);
            Action searchAct = () =>
            searchService.Search(new SearchParams { Url = "x.com", Keywords = new string[] { } });
            searchAct.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Search_Find_count_of_urls()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns("<div>x.com</div><div>myurl.com</div><div>y.com</div><div>myurl.com</div>");
            searchResultProvider.UrlInResultPattern.Returns("<div>{url}</div>");
            var searchService = new SearchService(this.searchResultProvider);
            var result = searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[0].Should().Be(3);
        }
    }
}
