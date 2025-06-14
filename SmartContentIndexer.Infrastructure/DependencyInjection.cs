using SmartContentIndexer.Core.Interfaces;
using SmartContentIndexer.Infrastructure.Data;
using SmartContentIndexer.Infrastructure.Repositories;
using SmartContentIndexer.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SmartContentIndexer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<SmartContentDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ??
                    "Data Source=SmartContentIndexer.db"));

            // Repositories
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IIndexingJobRepository, IndexingJobRepository>();

            // Services
            services.AddScoped<IContentExtractionService, ContentExtractionService>();
            services.AddSingleton<IFileSystemWatcher, FileSystemWatcherService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            return services;
        }
    }
}
