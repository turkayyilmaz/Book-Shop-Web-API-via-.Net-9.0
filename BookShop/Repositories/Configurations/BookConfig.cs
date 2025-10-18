using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Configurations
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(x => x.BookId);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.ISBN).IsRequired().HasMaxLength(13);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500);
            builder.HasOne(x => x.Category).WithMany(c => c.Books).HasForeignKey(x => x.CategoryId);
            builder.HasOne(x => x.Author).WithMany(a => a.Books).HasForeignKey(x => x.AuthorId);
        }
    }
}
