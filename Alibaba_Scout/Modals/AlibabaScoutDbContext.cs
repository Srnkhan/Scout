using Alibaba_Scout.Modals.Categories;
using Alibaba_Scout.Modals.Productions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Modals
{
    internal class AlibabaScoutDbContext : DbContext
    {
        public AlibabaScoutDbContext(DbContextOptions<AlibabaScoutDbContext> options)
            : base(options)
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Production>(b =>
            {
                b.HasOne<Category>(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId);
            });

        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Production> Productions { get; set; }
    }
}
