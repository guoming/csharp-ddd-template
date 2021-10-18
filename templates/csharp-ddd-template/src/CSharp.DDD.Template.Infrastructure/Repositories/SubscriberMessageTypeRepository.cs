using System.Collections.Generic;
using System.Threading.Tasks;
using Hummingbird.Extensions.UidGenerator;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Repositories;

namespace CSharp.DDD.Template.Infrastructure.Repositories
{
    public class SubscriberMessageTypeRepository : BaseRepository, ISubscriberMessageTypeRepository
    {
        public SubscriberMessageTypeRepository(IDbContext dbContext, IUniqueIdGenerator uniqueIdGenerator) : base(dbContext, uniqueIdGenerator)
        {
        }

        public async Task<bool> AddBatch(List<SubscriberMessageType> entitys)
        {
            List<BatchExecuteCommand> commands = new List<BatchExecuteCommand>();
            entitys.ForEach(item =>
            {
                item.id = this._uniqueIdGenerator.NewId();
                commands.Add(new BatchExecuteCommand()
                {
                    sql = "INSERT INTO subscriber_message_type (id,subscriber_id,message_type_id) VALUES(@id,@subscriber_id,@message_type_id)",
                    param = item
                });
            });
            return await this._dbContext.BatchExecuteAsync(commands);
        }

        public async Task<bool> Delete(long subscriber_id)
        {
            string sql = "DELETE FROM subscriber_message_type WHERE subscriber_id = @subscriber_id";
            return await this._dbContext.ExecuteAsync(sql, new { subscriber_id });
        }
    }
}
