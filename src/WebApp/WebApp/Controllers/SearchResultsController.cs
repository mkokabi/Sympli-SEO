using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public IEnumerable<SearchResult> Get()
        {
            return this.searchService.GetResults();
        }

        [HttpPost]
        public SearchResult Search([FromBody]SearchParams searchParams)
        {
            return this.searchService.Search(searchParams);
        }

        [HttpGet("echo")]
        public string Echo(string input)
        {
            return $"Echo {input}";
        }
    }
}