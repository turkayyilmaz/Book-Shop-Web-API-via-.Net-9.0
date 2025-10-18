using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public abstract record BookDtoForManipulation
    {
        // init; set; yaparsak sadece constructor'da set edilebilir, sonrasında edilemez.
        [Required(ErrorMessage = "Title alanı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Title Alanı en az 100 karakter olmalıdır.")]
        public string Title { get; init; }
        [MaxLength(500, ErrorMessage = "Description Alanı en az 500 karakter olmalıdır.")]
        public string Description { get; init; }
        [Range(1, 1000, ErrorMessage = "Price alanı 1 ile 1000 arasında olmalıdır.")]
        public decimal Price { get; init; }
        [Required(ErrorMessage = "ISBN alanı zorunludur.")]
        public string ISBN { get; init; }
        public DateTime PublishedDate { get; init; } = DateTime.Now;
        [Range(0, int.MaxValue, ErrorMessage = "Stock alanı 0 veya daha büyük bir değer olmalıdır.")]
        public int Stock { get; init; }
        public string? ImageUrl { get; init; }
        [Required(ErrorMessage = "CategoryId alanı zorunludur.")]
        public int CategoryId { get; init; }
        [Required(ErrorMessage = "AuthorId alanı zorunludur.")]
        public int AuthorId { get; init; }
    }
}
