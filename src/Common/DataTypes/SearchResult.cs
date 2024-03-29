﻿using System;

namespace Sympli.SEO.Common.DataTypes
{
    public class SearchResult
    {
        public Guid Id { get; set; }

        public int SearchEngineIndex { get; set; }

        public DateTime Date { get; set; }

        public string Url { get; set; }

        public string[] Keywords { get; set; }

        public int[] Results { get; set; }
    }
}
