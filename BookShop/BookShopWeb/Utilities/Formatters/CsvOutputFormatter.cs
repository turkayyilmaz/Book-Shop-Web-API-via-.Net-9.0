using Entities.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace BookShopWeb.Utilities.Formatters
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv")); // microsofttan çöz, system'dan değil
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        protected override bool CanWriteType(Type? type)
        {
            if (typeof(BookDto).IsAssignableFrom(type) || typeof(IEnumerable<BookDto>).IsAssignableFrom(type))
                return base.CanWriteType(type);
            return false;
        }
        private static void FormatCsv(StringBuilder buffer, BookDto book)
        {
            // ben hepsini yazmadım çünkü örnek amaçlı
            buffer.AppendLine($"\"{book.BookId}\",\"{book.Title}\",\"{book.AuthorId}\",\"{book.Description}\",{book.Price}");
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();
            if (context.Object is IEnumerable<BookDto> books)
            {
                foreach (var book in books)
                {
                    FormatCsv(buffer, book);
                }
            }
            else
            {
                var book = (BookDto)context.Object;
                FormatCsv(buffer, book);
            }
            await response.WriteAsync(buffer.ToString());
        }
    }
}
