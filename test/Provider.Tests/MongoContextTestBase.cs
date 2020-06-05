using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Tests
{
    public abstract class MongoContextTestBase : IDisposable
    {
        private const string MongoUrl = "mongodb://localhost:27017";

        protected IServiceProvider ServiceProvider;

        protected MongoContextTestBase()
        {
            ServiceProvider = new ServiceCollection()
                .AddDbContext<MongoContext>(options => options.UseMongo(MongoUrl, "families"))
                .BuildServiceProvider();

            ExecuteUnitOfWork(dbContext => dbContext.Database.EnsureCreated());
        }

        public void Dispose()
        {
            ExecuteUnitOfWork(dbContext => dbContext.Database.EnsureDeleted());
        }

        protected void ExecuteUnitOfWork(Action<MongoContext> unitOfWork)
        {
            using IServiceScope serviceScope = ServiceProvider.CreateScope();
            unitOfWork(serviceScope.ServiceProvider.GetService<MongoContext>());
        }

        protected async Task ExecuteUnitOfWorkAsync(Func<MongoContext, Task> unitOfWork)
        {
            using IServiceScope serviceScope = ServiceProvider.CreateScope();
            await unitOfWork(serviceScope.ServiceProvider.GetService<MongoContext>());
        }

        protected async Task<TResult> ExecuteUnitOfWorkAsync<TResult>(Func<MongoContext, Task<TResult>> unitOfWork)
        {
            using IServiceScope serviceScope = ServiceProvider.CreateScope();
            return await unitOfWork(serviceScope.ServiceProvider.GetService<MongoContext>());
        }
    }
}
