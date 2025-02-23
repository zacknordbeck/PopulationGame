using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public class UserGameLogRepository : IUserGameLogRepository
    {
        private readonly DapperContext _context;

        public UserGameLogRepository(DapperContext context)
        {
            _context = context;
        }

        public void CreateUserGameLog(UserGameLog log)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"
                    INSERT INTO UserGameLog (UserId, StartTime, EndTime, Streak, Guesses, Difficulty)
                    VALUES (@UserId, @StartTime, @EndTime, @Streak, @Guesses, @Difficulty);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                int id = connection.Query<int>(sql, log).Single();
                log.LogId = id;
            }
        }

        public IEnumerable<UserGameLog> GetGameLogsForUser(int userId)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"SELECT UserId, StartTime, EndTime, Streak, Guesses, Difficulty 
                            FROM UserGameLog 
                            WHERE UserId = @UserId ORDER BY StartTime DESC";

                return connection.Query<UserGameLog>(sql, new { UserId = userId }).ToList();
            }
        }

        public int GetGlobalHighScore()
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = "SELECT ISNULL(MAX(Streak), 0) FROM UserGameLog";
                return connection.QueryFirst<int>(sql);
            }
        }
    }
}
