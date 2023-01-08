using Alibaba_Scout.Modals.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public DbSet<Test> Tests { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
