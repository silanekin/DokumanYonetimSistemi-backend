using DokumanYonetimSistemi.DataModels;
using Microsoft.EntityFrameworkCore;


namespace DokumanYonetimSistemi.Repositories
{

    public class SqlUserRepository : IUserRepository
    {
        
        private  SystemAdminContext _context;
            public SqlUserRepository(SystemAdminContext context)
        {
            _context = context ?? throw new InvalidOperationException("DbContext başlatılmadı.");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        public void UpdateUser(DomainModels.User user)
        {
            throw new NotImplementedException();
        }
    }
}
