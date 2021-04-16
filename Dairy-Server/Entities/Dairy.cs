using System;
using System.Collections.Generic;

#nullable disable

namespace Dairy_Server.Entities
{
    public partial class Dairy
    {
        public int Uid { get; set; }
        public DateTime WroteDate { get; set; }
        public string Wheather { get; set; }
        public byte? Emotions { get; set; }
        public string Thema { get; set; }
        public string Content { get; set; }
        public bool Enabled { get; set; }
    }
}
