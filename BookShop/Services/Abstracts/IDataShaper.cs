using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString); //dynamic den gelecek ExpandoPnject
        ShapedEntity ShapeData(T entity, string fieldsString); //bu da tek gelecek nesne için
    }
}
