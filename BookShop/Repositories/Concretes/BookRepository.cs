using Entities.DTOs;
using Entities.Entities;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstracts;
using Repositories.Concretes.Extensions;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concretes
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        // base: üst sınıfın constructor'ını çağırır ve miras alınan sınıfın constructor'ına parametre geçer
        // AppDbContext context: dependency injection ile sağlanır yani context repositorybase'daki context atılır
        public BookRepository(AppDbContext context) : base(context)
        {

        }
        // IBookRepository'deki metotlar burada implemente edilir ve base'deki metotlara yönlendirilir
        // anlamsız gibi ama burada ek işlemler yapılabilir örnek olarak -|
        //public IQueryable<Book> GetAllBooks(bool trackChanges) => FindAll(trackChanges).Where(x => x.ISBN == "asd");
        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges)
        {
            var books = await FindAll(trackChanges)
                             .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
                             .Search(bookParameters.SearchTerm)
                             .Sort(bookParameters.OrderBy)
                             .ToListAsync();
            return PagedList<Book>.ToPagedList(books, bookParameters.PageNumber, bookParameters.PageSize);
        }

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(b => b.BookId)
                .ToListAsync();
        }
        public async Task<Book> GetOneBookByIdAsync(int id, bool trackChanges)
                        => await FindByCondition(x => x.BookId == id, trackChanges).FirstOrDefaultAsync();
        public void CreateOneBook(Book book) => Create(book);
        public void DeleteOneBook(Book book) => Delete(book);
        public void UpdateOneBook(Book book) => Update(book);

        public async Task<List<Book>> GetAllBooksAsyncV2(bool trackChanges)
        {
            return await FindAll(trackChanges).OrderBy(x => x.BookId).ToListAsync();
        }

        public async Task<IEnumerable<BookDto>> GetAllBookWithDetailsAsync(bool trachChanges)
        {
            return await _context.Books
                .OrderBy(x => x.BookId)
                .Select(b => new BookDto()
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Price = b.Price,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    PublishedDate = b.PublishedDate,
                    Stock = b.Stock,
                    ImageUrl = b.ImageUrl,
                    CategoryDto = new CategoryDto()
                    {
                        CategoryId = b.Category.CategoryId,
                        Name = b.Category.Name,
                        Description = b.Category.Description
                    },
                    AuthorDto = new AuthorDto()
                    {
                        AuthorId = b.Author.AuthorId,
                        FullName = b.Author.FullName,
                        Biography = b.Author.Biography,
                        Country = b.Author.Country,
                        BirthDate = b.Author.BirthDate,
                    }
                })
                .ToListAsync();
        }
    }
}
