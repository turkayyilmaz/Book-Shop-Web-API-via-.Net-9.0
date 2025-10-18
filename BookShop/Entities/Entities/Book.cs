using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Book
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
        public Category? Category { get; set; } // ? olması lazım aksi halde hata verir
        public int AuthorId { get; set; }
        public Author? Author { get; set; } // ? olması lazım aksi halde hata verir

    }
}
