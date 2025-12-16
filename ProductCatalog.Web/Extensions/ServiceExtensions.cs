using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repository;

namespace ProductCatalog.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceCollections(this IServiceCollection services)
        {
            AddDatabaseServices(services);
            AddCustomServices(services);
            AddLoggingServices(services);
            return services;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            services.AddScoped<DapperContext>();
            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Information);
            });
            return services;
        }
    }
}
