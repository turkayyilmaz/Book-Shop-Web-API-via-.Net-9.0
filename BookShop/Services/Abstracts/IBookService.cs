using Entities.DTOs;
using Entities.Entities;
using Entities.LinkModels;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstracts
{
    public interface IBookService
    {
        Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(LinkParameters linkParameters, bool trackChanges);
        Task<IEnumerable<BookDto>> GetAllBooksAsync(bool trackChanges);
        Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges);
        Task<BookDto> CreateOneBookAsync(BookDtoForCreate book);
        Task UpdateOneBookAsync(int id, BookDtoForUpdate book, bool trackChanges);
        Task DeleteOneBookAsync(int id, bool trackChanges);
        Task<(BookDtoForUpdate bookDto, Book Book)> GetOneBookForPatchAsync(int id, bool trackChanges);
        Task SaveChangesForPatchAsync(BookDtoForUpdate bookDto, Book book);
        Task<List<Book>> GetAllBooksAsyncV2(bool trackChanges);
        Task<IEnumerable<BookDto>> GetAllBookWithDetailsAsync(bool trachChanges);

    }
}
