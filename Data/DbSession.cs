using System.Data;
using System.Data.SqlTypes;
using ContactKeeper.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace ContactKeeper.Data
{
    
    public class DbSession : IDisposable
    {
       // private readonly DataContext context;

        // public IConfiguration Configuration { get; }
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public DbSession(IOptions<DatabaseSettings> databaseSettings)
        {
            var connectionString = databaseSettings.Value.ConnectionString;
            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

        public void BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction = null;
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction.Dispose();
            }
            Connection.Close();
            Connection.Dispose();
        }
    }
}