using dokumansistem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dokumansistem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _db;

        // Constructor, dependency injection ile UserContext alınır
        public UserRepository(UserContext db)
        {
            _db = db;
        }

        // Kullanıcıları listeleyen property
        public List<User> Users
        {
            get { return _db.Users.ToList(); }
        }

        // Kullanıcı oluşturma metodu
        public void CreateUser(User user)
        {
            _db.Users.Add(user); // Kullanıcıyı veritabanına ekler
            _db.SaveChanges();    // Değişiklikleri kaydeder
        }

        // Kullanıcıyı silme metodu
        public void DeleteUser(int userId)
        {
            var user = GetById(userId); // Kullanıcıyı Id'ye göre alır
            if (user != null)
            {
                _db.Users.Remove(user);  // Kullanıcıyı siler
                _db.SaveChanges();       // Değişiklikleri kaydeder
            }
            else
            {
                throw new Exception("User not found."); // Kullanıcı bulunamazsa hata fırlatılır
            }
        }

        // Kullanıcıyı Id ile bulma metodu
        public User GetById(int userId)
        {
            return _db.Users.FirstOrDefault(x => x.userId == userId) ?? throw new Exception("User not found.");
        }

        // Kullanıcıyı güncelleme metodu
        public void UpdateUser(User user)
        {
            _db.Users.Update(user); // Kullanıcıyı günceller
            _db.SaveChanges();      // Değişiklikleri kaydeder
        }
    }
}

