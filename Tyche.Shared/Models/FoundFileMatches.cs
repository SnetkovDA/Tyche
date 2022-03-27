using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyche.Shared.Models
{
    public class FoundFileMatches
    {
        public string ScannerId { get; set; }
        public string FileName { get; set; }
        public FoundMatch[] FoundMatches { get; set; }
    }
}
