using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public record BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto CategoryDto { get; set; }
        public int AuthorId { get; set; }
        public AuthorDto AuthorDto { get; set; }
    }
}
