using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);

        void CreateUser(User user);
    }
}
