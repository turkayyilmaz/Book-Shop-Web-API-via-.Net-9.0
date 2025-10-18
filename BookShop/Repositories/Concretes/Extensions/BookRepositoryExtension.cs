using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concretes.Extensions
{
    public static class BookRepositoryExtension
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, uint minPrice, uint maxPrice)
            => books.Where(b => b.Price >= minPrice && b.Price <= maxPrice);
        public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
        {
            if(string.IsNullOrEmpty(searchTerm))
                return books;
            var lowerCaseTerm = searchTerm.Trim().ToLower(); // KaRa => kara
            return books.Where(x => x.Title.Contains(searchTerm));
        }
        public static IQueryable<Book> Sort(this IQueryable<Book> books, string orderByQueryString)
        {
            if (string.IsNullOrEmpty(orderByQueryString))
                return books.OrderBy(x => x.BookId);

            // OrderQueryBuilder.CreateOrderQuery<T> T ye ister Book ve ister Category böylece global yaptık
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

            if (string.IsNullOrEmpty(orderQuery))
                return books.OrderBy(b => b.BookId);
            return books.OrderBy(orderQuery);
        }
    }
}
