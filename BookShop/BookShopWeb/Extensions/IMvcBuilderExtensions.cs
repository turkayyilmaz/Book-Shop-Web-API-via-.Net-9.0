using BookShopWeb.Utilities.Formatters;

namespace BookShopWeb.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddCsvFormatter(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options =>
            {
                options.OutputFormatters.Add(new CsvOutputFormatter());
            });
            return builder;
        }
    }
}
