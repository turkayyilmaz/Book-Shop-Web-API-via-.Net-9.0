using AspNetCoreRateLimit;
using BookShopWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Scalar.AspNetCore;
using Services.Abstracts;
using Services.Concretes;


var builder = WebApplication.CreateBuilder(args);

// Hocanýn yazdýðý kod bir alt satýrda, ancak Nlog 5.2 ile deðiþmiþ bu
//LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
LogManager.Setup().LoadConfigurationFromFile(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true; // Accept header'a saygý göster
    config.ReturnHttpNotAcceptable = true; // Accept header'da desteklenmeyen bir format varsa 406 döner
    config.CacheProfiles.Add("300SecondsDuration", new CacheProfile
    {
        Duration = 300
    });
})
    .AddXmlDataContractSerializerFormatters()
    .AddCsvFormatter() // kendi yazdýðýmýz Csv formatýný ekliyoruz
    .AddApplicationPart(typeof(Presentations.AssemblyReference).Assembly)
    .AddNewtonsoftJson(opt =>
               opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.ConfigureSwagger();
builder.Services.ConfigureActionFilters();

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true; // model validasyonunu controller'da yapacaðýz, otomatik yapmasýn
});
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(Program)); // tek satýrda çaðýrdýðýmýz için ayrý extension yazmadýk
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor(); // HttpContext'i service'lerde kullanabilmek için
//builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
var app = builder.Build();

// ihtiyacýmýz olan logger'ý alýyoruz
var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "BookShop v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "BookShop v2");
    });
}
if (app.Environment.IsProduction())
{
    // global olarak tüm isteklerde Https kullanýlýr
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting(); // rate limiting middleware, cors üzerinde olmalý
app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthentication(); // kimlik doðrulama authorizedan önce olmalý
app.UseAuthorization();

app.MapControllers();

app.Run();
