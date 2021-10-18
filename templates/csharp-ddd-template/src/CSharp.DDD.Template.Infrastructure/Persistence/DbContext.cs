using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Infrastructure.Configurations;

namespace CSharp.DDD.Template.Infrastructure.Persistence
{
    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private readonly ILogger<DbContext> logger;

        public DbContext(
            ILogger<DbContext> logger,
            IOptionsSnapshot<SqlConfiguration> options)
        {
            _connectionString = options.Value.ConnectionString;
            this.logger = logger;
        }

        public IDbConnection DbConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using (var conn = DbConnection())
            {
                logger.LogDebug(sql, param);
                return await conn.QueryAsync<T>(sql, param);
            }
        }


        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            using (var conn = DbConnection())
            {
                logger.LogDebug(sql, param);
                return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
            }
        }


        public async Task<bool> BatchExecuteAsync(List<BatchExecuteCommand> commands, IsolationLevel isolationLevel= IsolationLevel.RepeatableRead)
        {
            using (var conn = DbConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            logger.LogDebug(command.sql, command.param);
                            await conn.ExecuteAsync(command.sql, command.param);
                        }
                        trans.Commit();
                        return await Task.FromResult(true);
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> BatchExecuteAsync(List<BatchExecuteCommand> commands, IDbConnection conn, IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
        {
           
            if(conn.State!= ConnectionState.Open)
            {
                conn.Open();
            }

            using (var trans = conn.BeginTransaction(isolationLevel))
            {
                try
                {
                    foreach (var command in commands)
                    {
                        logger.LogDebug(command.sql, command.param);
                        await conn.ExecuteAsync(command.sql, command.param);
                    }
                    trans.Commit();
                    return await Task.FromResult(true);
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> ExecuteAsync(string sql, object param = null)
        {
            using (var conn = DbConnection())
            {
                logger.LogDebug(sql, param);
                return await conn.ExecuteAsync(sql, param) > 0;
            }
        }

        public async Task<bool> ExecuteAsync(string sql, object param = null, IDbConnection conn = null)
        {
            logger.LogDebug(sql, param);
            return await conn.ExecuteAsync(sql, param) > 0;
        }
    }
}
