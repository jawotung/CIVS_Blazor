using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class FinacleSVSContext : DbContext
    {
        public FinacleSVSContext(DbContextOptions<FinacleSVSContext> options)
            : base(options)
        {
        }

        public DbSet<SignCustInfo> SignCustInfo { get; set; }
        public DbSet<SignOtherInfo> SignOtherInfo { get; set; }
        public DbSet<SignMaintenance> SignMaintenance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SignCustInfo>(entity =>
            {
                entity.ToTable("signcustinfo", "finfadm"); 
                entity.HasKey(e => e.signid);
            });

            modelBuilder.Entity<SignOtherInfo>(entity =>
            {
                entity.ToTable("signotherinfo", "finfadm"); 
                entity.HasKey(e => e.signid);
            });

            modelBuilder.Entity<SignMaintenance>(entity =>
            {
                entity.ToTable("signmaintenance", "finfadm"); 
                entity.HasKey(e => e.signid);
            });
        }
    }
}
