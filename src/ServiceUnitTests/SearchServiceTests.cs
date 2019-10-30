using FluentAssertions;
using NSubstitute;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;
using Sympli.SEO.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ServiceUnitTests
{
    public class SearchServiceTests
    {
        private ISearchResultsProvider searchResultProvider;

        public SearchServiceTests()
        {
            searchResultProvider = Substitute.For<ISearchResultsProvider>();
            searchResultProvider.RemoveTralier(Arg.Any<String>()).Returns(callInfo => callInfo.Args()[0]);
        }

        [Fact]
        public void InvalidParameters_should_throw_exception_on_empty_url()
        {
            var searchService = new SearchService(this.searchResultProvider);
            Func<Task> searchAct = async () =>
                await searchService.Search(new SearchParams { Url = "", Keywords = new string[] { "a" } });
            searchAct.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void InvalidParameters_should_throw_exception_on_no_keywords()
        {
            var searchService = new SearchService(this.searchResultProvider);
            Func<Task> searchAct = async () =>
                await searchService.Search(new SearchParams { Url = "x.com", Keywords = new string[] { } });
            searchAct.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task Search_Find_count_of_urls_in_divs_flatAsync()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns("<div>x.com</div><div>myurl.com</div><div>y.com</div><div>myurl.com</div>");
            searchResultProvider.UrlInResultPattern.Returns("<div>{url}</div>");
            var searchService = new SearchService(this.searchResultProvider);
            var result = await searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[1].Should().Be(3);
        }

        [Fact]
        public async Task Search_Find_count_of_urls_in_divs_with_attr()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(
                @"<div at='1'>x.com</div><div at='1'>myurl.com</div><div at='1'>y.com</div><div at='1'>myurl.com</div>");
            searchResultProvider.UrlInResultPattern.Returns("<div at='1'>{url}</div>");
            var searchService = new SearchService(this.searchResultProvider);
            var result = await searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[1].Should().Be(3);
        }

        [Fact]
        public async Task Search_Find_count_of_urls_in_nested_divs_with_attr()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(@"<div at='1'><a l='x.com'></div>
<div at='1'><a l='myurl.com'></div>
<div at='1'><a l='y.com'></div>
<div at='1'><a l='myurl.com'></div>");
            searchResultProvider.UrlInResultPattern.Returns("<div at='1'><a l='{url}'></div>");
            var searchService = new SearchService(this.searchResultProvider);
            var result = await searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[1].Should().Be(3);
        }

        [Fact]
        public async Task Search_Find_count_of_urls_in_nested_divs_with_attr_double_quotes()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(@"<div at=""1""><a l=""x.com""></div>
<div at=""1""><a l=""myurl.com""></div>
<div at=""1""><a l=""y.com""></div>
<div at=""1""><a l=""myurl.com""></div>");
            searchResultProvider.UrlInResultPattern.Returns(@"<div at=""1""><a l=""{url}""></div>");
            var searchService = new SearchService(this.searchResultProvider);
            var result = await searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[1].Should().Be(3);
        }

        [Fact]
        public async Task Search_Find_count_of_urls_in_nested_divs_with_attr_double_quotes_with_inner_pattern()
        {
            searchResultProvider.SearchForKeywords(Arg.Any<string[]>()).Returns(@"<div at=""1""><a l=""x.com""></div>
<div at=""1""><a l=""myurl.com""></div>
<div at=""1""><a l=""y.com""></div>
<div at=""1""><a l=""myurl.com""></div>");
            searchResultProvider.UrlInResultPattern.Returns(@"<a l=""{url}"">");
            var searchService = new SearchService(this.searchResultProvider);
            var result = await searchService.Search(new SearchParams { Url = "myurl.com", Keywords = new string[] { "x", "y" } });
            result.Results.Should().HaveCount(2);
            result.Results[0].Should().Be(1);
            result.Results[1].Should().Be(3);
        }
    }
}
