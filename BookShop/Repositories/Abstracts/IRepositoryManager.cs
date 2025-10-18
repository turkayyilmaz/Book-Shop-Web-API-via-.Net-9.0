using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Abstracts
{
    public interface IRepositoryManager
    {
        IBookRepository Book { get; }
        ICategoryRepository Category { get; }

        //void Save(); // void tipinde bir dönüş tipi yok, bu yüzden Task olarak değiştirildi
        Task SaveAsync(); // Task<void> çalışmaz çünkü void bir tip değil, Sadece Task olarak yazılır
    }
}
