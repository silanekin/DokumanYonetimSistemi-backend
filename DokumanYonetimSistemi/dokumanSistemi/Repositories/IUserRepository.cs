using dokumansistem.Models;
using System.Collections.Generic;

namespace dokumansistem.Repositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
        List<User> Users { get; }
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
    }
}
