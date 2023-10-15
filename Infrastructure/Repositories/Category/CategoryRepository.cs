using Application.Interfaces.Category;
using Application.Interfaces.Customer;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Category
{
    public class CategoryRepository : RepositoryAsync<Domain.Entities.Category.Category, long>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
