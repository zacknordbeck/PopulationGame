using System.Data;
using System.Linq;
using Dapper;
using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public User GetUserByUsername(string username)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"SELECT UserId, Username 
                            FROM Users 
                            WHERE Username = @Username";

                return connection.QueryFirstOrDefault<User>(sql, new { Username = username });
            }
        }

        public void CreateUser(User user)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"INSERT INTO Users (Username) 
                            VALUES (@Username); 
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                int id = connection.Query<int>(sql, new { Username = user.Username }).Single();
                user.UserId = id;
            }
        }
    }
}
