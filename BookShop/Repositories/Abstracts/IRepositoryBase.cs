using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Abstracts
{
    public interface IRepositoryBase<T> // herhangi bir entity olabilir T
    {
        // CRUD operasyonları için temel metotlar
        IQueryable<T> FindAll(bool trackChanges); // trackChanges: değişiklikleri takip etme durumu
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges); // belirli bir koşula göre filtreleme
        void Create(T entity); // yeni bir varlık ekleme
        void Update(T entity); // varlığı güncelleme
        void Delete(T entity); // varlığı silme
    }
}
