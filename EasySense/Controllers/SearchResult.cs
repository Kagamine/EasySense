using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Controllers
{
    public class SearchResult
    {
        private dynamic data;
        private Pager pager;

        public SearchResult(dynamic data, Pager pager)
        {
            this.data = data;
            this.pager = pager;
        }

        public dynamic Data 
        {
            get
            {
                return data;
            }
        }

        public Pager Pager 
        {
            get
            {
                return pager;
            }
        }
    }
}