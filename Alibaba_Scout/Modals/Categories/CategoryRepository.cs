using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Modals.Categories
{
    internal class CategoryRepository : Repository<Category>, IRepository<Category>
    {
        public CategoryRepository(AlibabaScoutDbContext dbContext) : base(dbContext)
        {
        }
    }
}
