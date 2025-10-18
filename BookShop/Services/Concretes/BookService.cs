using AutoMapper;
using Entities.DTOs;
using Entities.Entities;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.RequestFeatures;
using Microsoft.VisualBasic;
using Repositories.Abstracts;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concretes
{
    public class BookService : IBookService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IBookLinks _bookLinks;
        private readonly ICategoryService _categoryService;
        public BookService(IRepositoryManager repository, ILoggerService logger, IMapper mapper, IBookLinks bookLinks, ICategoryService categoryService)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _bookLinks = bookLinks;
            _categoryService = categoryService;
        }
        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(LinkParameters linkParameters, bool trackChanges)
        {
            if(!linkParameters.BookParameters.ValidPriceRange)
                throw new PriceOutOfRangeException();
            var books = await _repository.Book.GetAllBooksAsync(linkParameters.BookParameters, trackChanges);
            var bookWithMetaData = _mapper.Map<IEnumerable<BookDto>>(books);

            var links = _bookLinks.TryGenerateLinks(bookWithMetaData,
                linkParameters.BookParameters.Fields,
                linkParameters.HttpContext);

            return (linkResponse: links, metaData: books.MetaData);
        }
        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(bool trackChanges)
        {
            var books = await _repository.Book.GetAllBooksAsync(trackChanges);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetOneBookByIdAndCheckIfItExists(id, trackChanges);
            return _mapper.Map<BookDto>(book);
        }
        public async Task<BookDto> CreateOneBookAsync(BookDtoForCreate book)
        {
            var category = await _categoryService.GetOneCategoryByIdAsync(book.CategoryId, false);
            var entity = _mapper.Map<Book>(book);
            _repository.Book.CreateOneBook(entity);
            await _repository.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }
        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            // check if book exists
            var book = await GetOneBookByIdAndCheckIfItExists(id, trackChanges);
            _repository.Book.DeleteOneBook(book);
            await _repository.SaveAsync();
        }
        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate book, bool trackChanges)
        {
            // check if book exists
            if (book == null)
            {
                throw new BookNotFoundException(id);
            }
            var bookToUpdate = await GetOneBookByIdAndCheckIfItExists(id, trackChanges);
            // update book mapping
            //bookToUpdate.Title = book.Title;
            //bookToUpdate.Description = book.Description;
            //bookToUpdate.Price = book.Price;
            //bookToUpdate.ISBN = book.ISBN;
            //bookToUpdate.PublishedDate = book.PublishedDate;
            //bookToUpdate.Stock = book.Stock;
            //bookToUpdate.ImageUrl = book.ImageUrl;
            //bookToUpdate.CategoryId = book.CategoryId;
            //bookToUpdate.AuthorId = book.AuthorId;
            _mapper.Map(book, bookToUpdate);
            _repository.Book.UpdateOneBook(bookToUpdate);
            await _repository.SaveAsync();
        }
        public async Task<(BookDtoForUpdate bookDto, Book Book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var book = await GetOneBookByIdAndCheckIfItExists(id, trackChanges);
            var bookDto = _mapper.Map<BookDtoForUpdate>(book);
            return (bookDto, book);
        }
        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDto, Book book)
        {
            _mapper.Map(bookDto, book);
            await _repository.SaveAsync();
        }
        public async Task<Book> GetOneBookByIdAndCheckIfItExists(int id, bool trackChanges)
        {
            var book = await _repository.Book.GetOneBookByIdAsync(id, trackChanges);
            if (book == null)
            {
                throw new BookNotFoundException(id);
            }
            return book;
        }
        public async Task<List<Book>> GetAllBooksAsyncV2(bool trackChanges)
        {
            var books = await _repository.Book.GetAllBooksAsyncV2(false);
            return books;
        }

        public async Task<IEnumerable<BookDto>> GetAllBookWithDetailsAsync(bool trachChanges)
        {
            return await _repository.Book.GetAllBookWithDetailsAsync(trachChanges);
        }
    }
}
