using DokumanYonetimSistemi.DomainModels;

namespace DokumanYonetimSistemi.Repositories
{
    public interface IUserRepository
    {
      
       Task<List<DokumanYonetimSistemi.DataModels.User>> GetUsersAsync();
    }
}
