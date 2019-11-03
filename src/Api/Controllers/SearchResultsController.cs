using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sympli.SEO.Common;
using Sympli.SEO.Common.DataTypes;
using Sympli.SEO.Common.Interfaces;

namespace Sympli.SEO.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchResultsController : ControllerBase
    {
        private readonly ILogger<SearchResultsController> logger;
        private readonly ISearchService searchService;

        public SearchResultsController(ILogger<SearchResultsController> logger, ISearchService searchService)
        {
            this.logger = logger;
            this.searchService = searchService;
        }

        [HttpGet]
        public async Task<PagedResponse<SearchResult>> Get(int startIndex, int pageSize)
        {
            return await this.searchService.GetResults(startIndex, pageSize);
        }

        [HttpPost]
        public async Task<SearchResult> Search([FromBody]SearchParams searchParams)
        {
            return await this.searchService.Search(searchParams);
        }

        [HttpGet("echo")]
        public string Echo(string input)
        {
            return $"Echo {input}";
        }
    }
}