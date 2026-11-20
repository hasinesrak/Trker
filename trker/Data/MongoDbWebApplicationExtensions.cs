using MongoDB.Bson;
using MongoDB.Driver;

namespace trker.Data
{
    public static class MongoDbWebApplicationExtensions
    {
        public static async Task VerifyMongoDbConnectionAsync(this WebApplication app)
        {
            try
            {
                var client = app.Services.GetRequiredService<IMongoClient>();

                await client.GetDatabase("admin")
                    .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));

                app.Logger.LogInformation("MongoDB connection verified.");
            }
            catch (Exception exception)
            {
                app.Logger.LogError(exception, "MongoDB connection could not be verified.");
            }
        }
    }
}
