using DokumanYonetimSistemi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DokumanYonetimSistemi.Controllers
{
    public class HomeController : Controller
    {
        private IUsersRepository _userRepository;

        public HomeController(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            return View(_userRepository.Users);
        }
    }
}
