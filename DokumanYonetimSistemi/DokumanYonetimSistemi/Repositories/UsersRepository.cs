using DokumanYonetimSistemi.DataModels;

namespace DokumanYonetimSistemi.Repositories
{
    public interface IUsersRepository
    {
        User GetById(int id);
        List<User> Users { get; }
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);


    }
    public class UsersRepository:IUsersRepository
    {
        private UserContext _db;

        public UsersRepository(UserContext db)
        {
            _db = db;
        }

        public List<User> Users
        {
            get { return _db.Users.ToList(); }

        }
        public void CreateUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var users = GetById(id);

            if (users != null)
            { 
                _db.Users.Remove(users);
            }
        }

        public User GetById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateUser(User user)
        {
            _db.Users.Update(user); 
            _db.SaveChanges();
        }
    }
}
