using Repositories.Abstracts;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concretes
{
    public class RepositoryManager : IRepositoryManager
    {
        // save işlemini context üzerinden yapacağız o yüzden DI yapıyoruz
        private readonly AppDbContext _context;
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        public RepositoryManager(AppDbContext context, IBookRepository bookRepository, ICategoryRepository categoryRepository)
        {
            _context = context;
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
        }
        // lazy yapısı sayesinde BookRepository nesnesi sadece ilk erişimde oluşturulur
        public IBookRepository Book => _bookRepository;
        public ICategoryRepository Category => _categoryRepository;
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
