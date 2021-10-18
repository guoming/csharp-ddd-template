using CSharp.DDD.Template.Domain.Core.Persistence;

namespace CSharp.DDD.Template.Infrastructure.Queries
{
    public abstract class BaseQuery
    {
        public readonly IDbContext _dbContext;

        public BaseQuery(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


    }
}
