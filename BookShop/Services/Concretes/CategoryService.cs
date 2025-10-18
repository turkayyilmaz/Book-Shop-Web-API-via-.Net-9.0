using Entities.Entities;
using Entities.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Repositories.Abstracts;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concretes
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryManager _repositoryManager;
        public CategoryService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges)
        {
             return await _repositoryManager.Category.GetAllCategoriesAsync(trackChanges);
        }
        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges)
        {
            var category =  await _repositoryManager.Category.GetOneCategoryByIdAsync(id, trackChanges);
            if (category == null) 
                throw new CategoryNotFoundException(id);
            return category;
        }
    }
}
