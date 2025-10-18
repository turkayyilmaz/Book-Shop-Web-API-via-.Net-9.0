using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        // protected yapmamızın sebebi abstract class newlenemediği için ctor yapılamaz. Sadece, kalıtım yapan classlar
        // kullanabilecek
        protected NotFoundException(string message) : base(message)
        {
            
        }
    }
}
