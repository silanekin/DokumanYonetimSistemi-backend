using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokumanSistem.Core.Models;

namespace DokumanSistem.Core.Interfaces
{
        public interface IUserRepository
        {
            Task<User> GetByIdAsync(int id);
            Task<IEnumerable<User>> GetAllAsync();
            Task AddAsync(User user, string createdBy);
            Task UpdateAsync(User user, string updatedBy);
            Task DeleteAsync(int id);
        }
    }


