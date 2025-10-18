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
    // abstract: bu sınıftan nesne oluşturulamaz ama miras alınabilir 
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        // protected: sadece bu sınıf ve miras alan sınıflar erişebilir
        protected readonly AppDbContext _context;
        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<T> FindAll(bool trackChanges) => !trackChanges ? 
            _context.Set<T>().AsNoTracking() : // değişiklikleri takip etme
            _context.Set<T>(); // değişiklikleri takip et
        public IQueryable<T> FindByCondition(System.Linq.Expressions.Expression<Func<T, bool>> expression, bool trackChanges)
                => 
            !trackChanges ? _context.Set<T>().Where(expression).AsNoTracking() : _context.Set<T>().Where(expression);
        public void Create(T entity) => _context.Set<T>().Add(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
    }
}
