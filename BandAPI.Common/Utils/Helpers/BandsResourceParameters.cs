﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandAPI.Helpers
{
    public class BandsResourceParameters
    {
        public string SearchQuery { get; set; }

        const int maxPageSize = 13;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 13;

        public int PageSize { get => _pageSize; set => _pageSize = (value > maxPageSize) ? maxPageSize : value; }

        public string OrderBy { get; set; } = "Name";
    }
}
