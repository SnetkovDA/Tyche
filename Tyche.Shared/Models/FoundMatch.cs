using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyche.Shared.Models
{
    public class FoundMatch
    {
        public string Name { get; set; }
        public string[] Matches { get; set; }
        public int MatchesCount { get; set; }
    }
}
