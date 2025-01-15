using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class PRDSVSContext : DbContext
    {
        public PRDSVSContext(DbContextOptions<PRDSVSContext> options)
            : base(options)
        {
        }

        public DbSet<CustInfo> CustInfo { get; set; }
        public DbSet<SvsGroup> SvsGroup { get; set; }
        public DbSet<SvsRule> SvsRule { get; set; }
        public DbSet<Image> Image { get; set; }

    }
}
