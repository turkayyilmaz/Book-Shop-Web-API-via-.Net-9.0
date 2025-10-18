using AspNetCoreRateLimit;
using Entities.DTOs;
using Entities.Entities;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentations.ActionFilters;
using Presentations.Controllers;
using Repositories.Abstracts;
using Repositories.Concretes;
using Repositories.Data;
using Services.Abstracts;
using Services.Concretes;
using System.Text;
namespace BookShopWeb.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContext<AppDbContext>(opitons =>
           opitons.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        // burada service manager'ı ekliyoruz
        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        // burada nlog'u ekliyoruz
        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerService>();
        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>(); // scoped : her istek için yeni nesne, dto doğrulama için uygun
            services.AddSingleton<LogFilterAttribute>(); // singleton : uygulama boyunca tek nesne, log için uygun
            services.AddScoped<ValidateMediaTypeAttribute>();
        }
        // burada cors'u ekliyoruz
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")
                    );
            });
        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config
               .OutputFormatters
               .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+json");

                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+xml");

                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+xml");
                }
            }
            );
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true; // response header'ında desteklenen versiyonları gösterir
                opt.AssumeDefaultVersionWhenUnspecified = true; // versiyon belirtilmemişse default versiyonu kullan
                opt.DefaultApiVersion = new ApiVersion(1, 0); // default versiyon 1.0
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<BookController>()
                    .HasApiVersion(new ApiVersion(1, 0));

                opt.Conventions.Controller<BookV2Controller>()
                    .HasDeprecatedApiVersion(new ApiVersion(2, 0));
                // opt.ApiVersionReader = new MediaTypeApiVersionReader("ver");
                // combine kullanarak birden fazla versiyon okuma şekli belirleyebiliriz
                // opt.ApiVersionReader = ApiVersionReader.Combine(
                //     new HeaderApiVersionReader("api-version"),
                //     new MediaTypeApiVersionReader("ver")
                // );
            });
        }
        public static void ConfigureResponseCaching(this IServiceCollection services)
        {
            services.AddResponseCaching();
        }
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders(expirationOpt =>
            {
                expirationOpt.MaxAge = 65; // 65 saniye cache'te tut
                expirationOpt.CacheLocation = CacheLocation.Private; // cache sadece client'ta tutulacak
            },
            validationOpt =>
            {
                validationOpt.MustRevalidate = false; // cache'in süresi dolduğunda yeniden doğrulama yapma!
            }
            );
        }
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*", // tüm endpoint'ler için geçerli
                    Limit = 3, // 3 istek
                    Period = "1m" // 1 dakika içinde
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>() // Identity için AppDbContext kullan
                .AddDefaultTokenProviders(); // password reset, email confirmation gibi işlemler için token sağlayıcı ekle
        }
        //public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var jwtSettings = configuration.GetSection("JwtSettings");
        //    var secretKey = jwtSettings["secretKey"];
        //    services.AddAuthentication(opt =>
        //    {
        //        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(options =>
        //    {
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = jwtSettings["validIssuer"],
        //            ValidAudience = jwtSettings["validAudience"],
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        //        };
        //    });
        //}
        // GEMİNİ TARAFINDAN AÇIKLAMA SATIRLARI EKLENDİ
        // IServiceCollection arayüzünü genişleten (extension method) bir metot tanımlıyoruz.
        // Bu metot, JWT (JSON Web Token) ayarlarını yapılandırmak için kullanılacak.
        // 'this' anahtar kelimesi, bu metodun IServiceCollection üzerinde çağrılabileceğini belirtir.
        // 'configuration' parametresi, appsettings.json gibi yapılandırma dosyalarından veri okumak için kullanılır.
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Yapılandırma dosyasından (appsettings.json) "JwtSettings" isimli bölümü alıyoruz.
            // Bu bölümde token için gerekli olan issuer, audience, secretKey gibi bilgiler bulunur.
            var jwtSettings = configuration.GetSection("JwtSettings");

            // "JwtSettings" bölümünün içinden "secretKey" değerini string olarak alıyoruz.
            // Bu anahtar, token'ı imzalamak ve doğrulamak için kullanılan gizli anahtardır.
            var secretKey = jwtSettings["secretKey"];

            // Servis koleksiyonuna kimlik doğrulama (Authentication) servisini ekliyoruz.
            // Lambda (=>) ifadesiyle varsayılan ayarları yapılandırıyoruz.
            services.AddAuthentication(opt =>
            {
                // Varsayılan kimlik doğrulama şemasını belirliyoruz.
                // Bir kullanıcı kimliğini doğrulamaya çalışırken bu şema kullanılır.
                // Genellikle değeri "Bearer"dır.
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                // Varsayılan "challenge" şemasını belirliyoruz.
                // Kimliği doğrulanmamış bir kullanıcı, yetki gerektiren bir kaynağa erişmeye çalıştığında
                // bu şema devreye girer ve genellikle 401 Unauthorized yanıtı döndürülmesini sağlar.
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JWT Bearer kimlik doğrulama "handler"ını (işleyicisini) ekliyoruz.
            // "Bearer" şeması için token'ın nasıl doğrulanacağını burada belirtiyoruz.
            .AddJwtBearer(options =>
            {
                // Gelen token'ların hangi kurallara göre doğrulanacağını belirten parametreleri ayarlıyoruz.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Token'ı yayınlayanın (issuer) doğrulanıp doğrulanmayacağını belirtir. (true = doğrula)
                    ValidateIssuer = true,
                    // Token'ın hedef kitlesinin (audience) doğrulanıp doğrulanmayacağını belirtir. (true = doğrula)
                    ValidateAudience = true,
                    // Token'ın yaşam süresinin (lifetime) kontrol edilip edilmeyeceğini belirtir. (true = kontrol et)
                    // Yani token'ın süresinin geçip geçmediğine bakılır.
                    ValidateLifetime = true,
                    // Token'ı imzalayan anahtarın (signing key) doğrulanıp doğrulanmayacağını belirtir. (true = doğrula)
                    ValidateIssuerSigningKey = true,

                    // Geçerli token yayınlayıcısının (issuer) kim olduğunu belirtir.
                    // Bu değer, appsettings.json dosyasından alınır. Gelen token'daki 'iss' claim'i bu değerle eşleşmelidir.
                    ValidIssuer = jwtSettings["validIssuer"],
                    // Geçerli token hedef kitlesinin (audience) kim olduğunu belirtir.
                    // Bu değer, appsettings.json dosyasından alınır. Gelen token'daki 'aud' claim'i bu değerle eşleşmelidir.
                    ValidAudience = jwtSettings["validAudience"],
                    // Token'ı doğrulamak için kullanılacak güvenlik anahtarını belirtir.
                    // appsettings.json'dan alınan 'secretKey' string'i, UTF8 formatında byte dizisine çevrilir
                    // ve simetrik bir güvenlik anahtarı (SymmetricSecurityKey) oluşturulur.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BookShop",
                    Version = "v1",
                    Description = "BookShop Web Api is created by following BTK Course",
                    TermsOfService = new Uri("https://www.btkakademi.gov.tr"),
                    Contact = new OpenApiContact
                    {
                        Name = "Türkay Yılmaz",
                        Email = "mail@gmail.com",
                        Url = new Uri("https://www.btkakademi.gov.tr")
                    }
                });

                s.SwaggerDoc("v2", new OpenApiInfo { Title = "BookShop", Version = "v2" });
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                        Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });
        }
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthenticationServices, AuthenticationService>();
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}
