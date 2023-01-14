using Alibaba_Scout.Modals.Categories;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Modals.Productions
{
    internal class ProductionRepository : Repository<Production>, IRepository<Production>
    {
        public ProductionRepository(AlibabaScoutDbContext dbContext) : base(dbContext)
        {
        }
    }
}
