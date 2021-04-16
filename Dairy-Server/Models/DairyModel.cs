using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dairy_Server.Models
{
    public class DairyModel : Entities.Dairy
    {
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
