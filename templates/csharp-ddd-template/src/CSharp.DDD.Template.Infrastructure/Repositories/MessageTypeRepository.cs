using System.Threading.Tasks;
using Hummingbird.Extensions.UidGenerator;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Repositories;

namespace CSharp.DDD.Template.Infrastructure.Repositories
{
    public class MessageTypeRepository : BaseRepository, IMessageTypeRepository
    {
        public MessageTypeRepository(IDbContext dbContext, IUniqueIdGenerator uniqueIdGenerator) : base(dbContext, uniqueIdGenerator)
        {
        }

        public async Task<MessageType> Add(MessageType entity)
        {
            string sql = "INSERT INTO message_type (id,code,name) VALUES(@id,@code,@name)";
            entity.id = this._uniqueIdGenerator.NewId();

            if (!await this._dbContext.ExecuteAsync(sql, entity))
            {
                entity.id = 0;
            }
            return entity;
        }
    }
}
