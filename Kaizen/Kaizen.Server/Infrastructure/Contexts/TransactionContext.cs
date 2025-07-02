using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Contexts
{
    public class TransactionContext : IDisposable
    {
        public SqlConnection Connection { get; private set; }
        public SqlTransaction Transaction { get; private set; }

        public TransactionContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string cannot be null or empty.");
            Connection = new SqlConnection(connectionString);
        }

        public async Task OpenAsync(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
        {
            await Connection.OpenAsync();
            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        public async Task CommitAsync()
        {
            if (Transaction != null)
            {
                await Transaction.CommitAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (Transaction != null)
            {
                await Transaction.RollbackAsync();
            }
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}