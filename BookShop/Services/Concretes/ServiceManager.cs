using AutoMapper;
using Entities.DTOs;
using Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Repositories.Abstracts;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concretes
{
    public class ServiceManager : IServiceManager
    {
        private readonly IBookService _bookService;
        private readonly IAuthenticationServices _authenticationServices;
        private readonly ICategoryService _categoryService;
        public ServiceManager(IBookService bookService, IAuthenticationServices authenticationServices, ICategoryService categoryService)
        {
            _bookService = bookService;
            _authenticationServices = authenticationServices;
            _categoryService = categoryService;
        }
        // private readonly kullanmadık çünkü sadece property üzerinden erişilecek
        public IBookService BookService => _bookService;
        public IAuthenticationServices AuthenticationServices => _authenticationServices;
        public ICategoryService CategoryService => _categoryService;
    }
}
