using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Models.ViewModels
{
    public class PageVM
    {
        public IEnumerable<Page> Pages { get; set; }
        public IEnumerable<News> News { get; set; }
    }
}
