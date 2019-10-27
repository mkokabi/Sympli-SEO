using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchResultsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<SearchResultsController> logger;

        public SearchResultsController(ILogger<SearchResultsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<SearchResult> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new SearchResult
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("echo")]
        public string Echo(string input)
        {
            return $"Echo {input}";
        }
    }
}