using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public interface IUserRepository
    {
        // Hämtar en användare baserat på användarnamn. Returnerar null om användaren inte finns.
        User GetUserByUsername(string username);

        // Skapar en ny användare och sätter användarens id efter att den sparats.
        void CreateUser(User user);
    }
}
