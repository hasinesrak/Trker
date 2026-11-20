using Microsoft.Extensions.Options;
using MongoDB.Driver;
using trker.Configuration;

namespace trker.Data
{
    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<MongoDbSettings>()
                .Bind(configuration.GetSection(MongoDbSettings.SectionName))
                .Validate(
                    settings => !string.IsNullOrWhiteSpace(settings.ConnectionString),
                    "MongoDB connection string is missing. Set MONGODB_URI in .env or add MongoDb:ConnectionString to configuration.")
                .Validate(
                    settings => !string.IsNullOrWhiteSpace(settings.DatabaseName),
                    "MongoDB database name is missing. Set MONGODB_DATABASE_NAME in .env or add MongoDb:DatabaseName to configuration.")
                .ValidateOnStart();

            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;

                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = serviceProvider.GetRequiredService<IMongoClient>();

                return client.GetDatabase(settings.DatabaseName);
            });

            return services;
        }
    }
}
