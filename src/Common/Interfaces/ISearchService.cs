using Sympli.SEO.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sympli.SEO.Common.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<SearchResult> GetResults();
        SearchResult Search(SearchParams searchParams);
    }
}
