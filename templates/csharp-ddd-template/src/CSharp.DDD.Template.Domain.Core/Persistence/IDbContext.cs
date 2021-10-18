using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CSharp.DDD.Template.Domain.Core.Persistence
{
    public class BatchExecuteCommand
    {
        public string sql { get; set; }
        public object param { get; set; }
    }

    public interface IDbContext
    {
        IDbConnection DbConnection();

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);

        Task<bool> BatchExecuteAsync(List<BatchExecuteCommand> commands, IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);

        Task<bool> BatchExecuteAsync(List<BatchExecuteCommand> commands, IDbConnection conn, IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);
        Task<bool> ExecuteAsync(string sql, object param = null);
        Task<bool> ExecuteAsync(string sql, object param = null, IDbConnection conn = null);
       
    }
}