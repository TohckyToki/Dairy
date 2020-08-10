using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dairy.Entities {
    public class Dairy {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Uid { get; set; }

        [MaxLength(20)]
        public string Thema { get; set; }

        [Required]
        public DateTime WroteDate { get; set; }

        [MaxLength(10)]
        public string Wheather { get; set; }

        public string Content { get; set; }

        [Required]
        public bool Enable { get; set; }
    }
}
