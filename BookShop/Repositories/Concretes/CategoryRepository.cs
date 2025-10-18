using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstracts;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concretes
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges) =>
            await FindAll(trackChanges).OrderBy(c => c.CategoryId).ToListAsync();
        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges) =>
            await FindByCondition(x => x.CategoryId == id, trackChanges).FirstOrDefaultAsync();
        public void CreateCategory(Category category) => Create(category);
        public void UpdateCategory(Category category) => Update(category);
        public void DeleteCategory(Category category) => Delete(category);
    }
}
