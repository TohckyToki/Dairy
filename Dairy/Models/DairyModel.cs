using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dairy.Models
{
    public class DairyModel
    {
        public int Uid { get; set; }
        public DateTime WroteDate { get; set; }
        public string Wheather { get; set; }
        public byte? Emotions { get; set; }
        public string Thema { get; set; }
        public string Content { get; set; }
        public bool Enabled { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
