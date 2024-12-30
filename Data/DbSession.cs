using System.Data;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace ContactKeeper.Data
{
    
    public class DbSession : IDisposable
    {
        
        public IConfiguration Configuration { get; }
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public DbSession(IConfiguration configuration)
        {
            Configuration = configuration;
            Connection = new SqlConnection(Configuration.GetConnectionString("connectionString"))  ;
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