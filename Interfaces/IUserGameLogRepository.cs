using System.Collections.Generic;
using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public interface IUserGameLogRepository
    {
        // Skapar en ny spelomgångslogg och sätter loggens id
        void CreateUserGameLog(UserGameLog log);

        // Hämtar alla spelomgångar för en specifik användare (sorterade på starttid, nyaste först)
        IEnumerable<UserGameLog> GetGameLogsForUser(int userId);

        // Hämtar den globala högsta streaken från alla spelomgångar
        int GetGlobalHighScore();
    }
}
