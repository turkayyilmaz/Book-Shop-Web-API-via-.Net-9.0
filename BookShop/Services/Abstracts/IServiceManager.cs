using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstracts
{
    public interface IServiceManager
    {
        IBookService BookService { get;  }
        IAuthenticationServices AuthenticationServices { get; }
        ICategoryService CategoryService { get; }
    }
}
