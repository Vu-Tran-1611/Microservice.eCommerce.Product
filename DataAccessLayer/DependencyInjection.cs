using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsMicroService.DataAccessLayer.Context;
using ProductsMicroService.DataAccessLayer.Repository;
using ProductsMicroService.DataAccessLayer.RepositoryContracts;

namespace ProductsMicroService.DataAccessLayer;


public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services,IConfiguration configuration)
    {
        // Register your data access layer services here
        // Example: services.AddScoped<IProductRepository, ProductRepository>();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySQL(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }
}
