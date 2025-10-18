using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }
        // int count: veritabanındaki toplam kayıt sayısı
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            // OOP'de en çok yapılan hata: MetaData'yı new'lememek genellikle unutuluyor ve ctor içinde new'lenir genelde.
            MetaData = new MetaData
            {
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PageSize = pageSize,
                TotalCount = count
            };
            AddRange(items);
        }
        // source: veritabanındaki tüm kayıtlar count: bunu burda yazmadık çünkü source'dan alacağız
        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
