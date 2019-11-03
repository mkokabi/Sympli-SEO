using System;
using System.Collections.Generic;
using System.Text;

namespace Sympli.SEO.Common
{
    public class PagedResponse<EntityType>
    {
        public int Length { get; set; }
        public IEnumerable<EntityType> Results { get; set; }
    }
}
