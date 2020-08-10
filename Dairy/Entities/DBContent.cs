using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dairy.Entities {
    public class EntiryManager: DbContext {
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=DataConfig.dll");

        public DbSet<Dairy> Dairies { get; set; }
    }
}
