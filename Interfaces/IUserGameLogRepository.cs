using System.Collections.Generic;
using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public interface IUserGameLogRepository
    {
        void CreateUserGameLog(UserGameLog log);

        IEnumerable<UserGameLog> GetGameLogsForUser(int userId);

        int GetGlobalHighScore();
    }
}
