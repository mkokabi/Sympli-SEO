using System;
using System.Collections.Generic;
using System.Text;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchResultsProvider
    {
        string SearchForKeywords(string[] keywords);
        string UrlInResultPattern { get; }
    }
}
