using Hummingbird.Extensions.UidGenerator;
using CSharp.DDD.Template.Domain.Core.Persistence;

namespace CSharp.DDD.Template.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        public readonly IDbContext _dbContext;
        public readonly IUniqueIdGenerator _uniqueIdGenerator;

        public BaseRepository(IDbContext dbContext, IUniqueIdGenerator uniqueIdGenerator)
        {
            this._dbContext = dbContext;
            this._uniqueIdGenerator = uniqueIdGenerator;
        }
    }
}
