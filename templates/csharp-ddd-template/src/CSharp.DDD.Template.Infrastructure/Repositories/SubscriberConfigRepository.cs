using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hummingbird.Extensions.Cache;
using Hummingbird.Extensions.UidGenerator;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Repositories;

namespace CSharp.DDD.Template.Infrastructure.Repositories
{
    public class SubscriberConfigRepository : BaseRepository, ISubscriberConfigRepository
    {
        private static object _async = new object();
        private readonly IHummingbirdCache<object> _hummingbirdCache;

        public SubscriberConfigRepository(IDbContext dbContext, IUniqueIdGenerator uniqueIdGenerator, IHummingbirdCache<object> hummingbirdCache) : base(dbContext, uniqueIdGenerator)
        {
            this._hummingbirdCache = hummingbirdCache;
        }

        public async Task<SubscriberConfig> Add(SubscriberConfig entity, List<MessageType> messageTypes, List<MessageType> insertMessageTypes)
        {
            entity.id = this._uniqueIdGenerator.NewId();
            entity.message_channel_key = this._uniqueIdGenerator.NewId().ToString();
            entity.add_time = DateTime.UtcNow;
            entity.update_time = DateTime.UtcNow;

            var commands = new List<BatchExecuteCommand>();
            commands.Add(new BatchExecuteCommand
            {
                sql = "INSERT INTO subscriber_config (id,message_channel_key,app_token,app_key,url,add_time,update_time) VALUES(@id,@message_channel_key,@app_token,@app_key,@url,@add_time,@update_time)",
                param = entity
            });

            //新增消息类型枚举
            insertMessageTypes?.ForEach(item =>
            {
                var messageType = new MessageType
                {
                    id = this._uniqueIdGenerator.NewId(),
                    code = item.code,
                    name = item.name
                };
                commands.Add(new BatchExecuteCommand
                {
                    sql = "INSERT INTO message_type (id,code,name) VALUES(@id,@code,@name)",
                    param = messageType
                });
                messageTypes.Add(messageType);
            });

            //插入订阅者与消息类型关联关系
            messageTypes?.ForEach(item =>
            {
                commands.Add(new BatchExecuteCommand()
                {
                    sql = "INSERT INTO subscriber_message_type (id,subscriber_id,message_type_id) VALUES(@id,@subscriber_id,@message_type_id)",
                    param = new SubscriberMessageType
                    {
                        id = this._uniqueIdGenerator.NewId(),
                        message_type_id = item.id,
                        subscriber_id = entity.id
                    }
                });
            });

            if (!await this._dbContext.BatchExecuteAsync(commands))
            {
                entity = null;
            }

            return entity;
        }

        public async Task<bool> Update(SubscriberConfig entity, List<MessageType> messageTypes, List<MessageType> insertMessageTypes)
        {
            entity.update_time = DateTime.UtcNow;

            var commands = new List<BatchExecuteCommand>();
            commands.Add(new BatchExecuteCommand
            {
                sql = @" UPDATE subscriber_config 
                            SET
                                app_token = @app_token,
                                app_key = @app_key,
                                url = @url,
                                update_time = @update_time
                            WHERE message_channel_key = @message_channel_key",
                param = entity
            });
            _hummingbirdCache.Delete($"{typeof(SubscriberConfig).Name}_{entity.message_channel_key}", $"{typeof(SubscriberConfig).Name}");

            //新增消息类型枚举
            insertMessageTypes?.ForEach(item =>
            {
                var messageType = new MessageType
                {
                    id = this._uniqueIdGenerator.NewId(),
                    code = item.code,
                    name = item.name
                };
                commands.Add(new BatchExecuteCommand
                {
                    sql = "INSERT INTO message_type (id,code,name) VALUES(@id,@code,@name)",
                    param = messageType
                });
                messageTypes.Add(messageType);
            });
            _hummingbirdCache.Delete($"{typeof(MessageType).Name}_All", $"{typeof(MessageType).Name}");

            //删除历史消息类型绑定关系
            commands.Add(new BatchExecuteCommand
            {
                sql = "DELETE FROM subscriber_message_type WHERE subscriber_id = @subscriber_id",
                param = new { subscriber_id = entity.id }
            });

            //插入订阅者与消息类型关联关系
            messageTypes?.ForEach(item =>
            {
                commands.Add(new BatchExecuteCommand()
                {
                    sql = "INSERT INTO subscriber_message_type (id,subscriber_id,message_type_id) VALUES(@id,@subscriber_id,@message_type_id)",
                    param = new SubscriberMessageType
                    {
                        id = this._uniqueIdGenerator.NewId(),
                        message_type_id = item.id,
                        subscriber_id = entity.id
                    }
                });
                _hummingbirdCache.Delete($"{typeof(SubscriberMessageType).Name}_{entity.id}_{item.id}", $"{typeof(SubscriberMessageType).Name}");
            });
            return await this._dbContext.BatchExecuteAsync(commands);
        }
    }
}
