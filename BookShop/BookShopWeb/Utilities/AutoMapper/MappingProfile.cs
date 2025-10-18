using AutoMapper;
using Entities.DTOs;
using Entities.Entities;

namespace BookShopWeb.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            // CreateMap<Source, Target>();
            CreateMap<BookDtoForUpdate, Book>().ReverseMap(); // iki yönlü dönüşüm, aynısından tersini yaptığını yazmana gerek yok
            CreateMap<Book, BookDto>();
            CreateMap<BookDtoForCreate, Book>();
            CreateMap<User, UserForRegistrationDto>().ReverseMap();
        }
    }
}
