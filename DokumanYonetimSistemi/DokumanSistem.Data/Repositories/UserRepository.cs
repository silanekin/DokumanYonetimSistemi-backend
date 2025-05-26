using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokumanSistem.Core.Interfaces;
using DokumanSistem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DokumanSistem.Data.Repositories
{
        public class UserRepository : IUserRepository
        {
            private readonly DocumentDbContext _context;

            public UserRepository(DocumentDbContext context)
            {
                _context = context;
            }

            public async Task<User> GetByIdAsync(int id)
            {
                return await _context.Users.FindAsync(id);
            }

            public async Task<IEnumerable<User>> GetAllAsync()
            {
                return await _context.Users.ToListAsync();
            }

            public async Task AddAsync(User user, string createdBy)
            {
                user.CreatedBy = createdBy;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(User user, string updatedBy)
            {
                user.UpdatedBy = updatedBy;
                user.UpdatedDate = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var user = await GetByIdAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }

        Task<User> IUserRepository.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<User>> IUserRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

       
    }
    }


