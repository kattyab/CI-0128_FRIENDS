using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Contexts
{
    public class PayrollTransactionContext : IDisposable
    {
        public SqlConnection Connection { get; private set; }
        public SqlTransaction Transaction { get; private set; }

        public PayrollTransactionContext(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        public async Task OpenAsync()
        {
            await Connection.OpenAsync();
            Transaction = Connection.BeginTransaction(IsolationLevel.Serializable);
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
