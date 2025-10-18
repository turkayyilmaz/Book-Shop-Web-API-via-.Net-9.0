using Entities.DTOs;
using Entities.Entities;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Abstracts
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
        // özel metotlar burada tanımlanır, aynıları var zaten base'de ama ne amaçlı kullanılıyor diye burada yazdık
        Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges);
        Task<List<Book>> GetAllBooksAsync(bool trackChanges);
        Task<Book> GetOneBookByIdAsync(int id, bool trackChanges);
        void CreateOneBook(Book book);
        void UpdateOneBook(Book book);
        void DeleteOneBook(Book book);
        Task<IEnumerable<BookDto>> GetAllBookWithDetailsAsync(bool trachChanges);
        Task<List<Book>> GetAllBooksAsyncV2(bool trackChanges);
    }
}
