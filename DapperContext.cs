using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PopulationGame
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PopulationGame");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class SqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqlDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
