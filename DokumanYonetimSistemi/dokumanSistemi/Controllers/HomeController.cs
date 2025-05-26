using System.Diagnostics;
using dokumansistem.Models;
using dokumansistem.Repositories;
using dokumanSistemi.Models;
using Microsoft.AspNetCore.Mvc;

namespace dokumanSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<User> _userRepository;

        public HomeController(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // Kullanýcýlarý listeleme iþlemi
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();  // Kullanýcýlarý çekiyoruz
            return View(users);
        }
        // Yeni kullanýcý eklemek için
        public IActionResult Create()
        {
            return View();
        }

        // Kullanýcýyý eklemek için POST iþlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.AddAsync(user);  // Yeni kullanýcýyý ekliyoruz
                return RedirectToAction("Index");
            }
            return View(user); // Hata varsa ayný sayfada göster
        }

        // Kullanýcýyý silme iþlemi
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);  // Kullanýcýyý getiriyoruz
            if (user == null)
            {
                return NotFound();  // Kullanýcý bulunmazsa hata döner
            }

            await _userRepository.DeleteAsync(userId);  // Kullanýcýyý silme iþlemi
            return RedirectToAction("Index");  // Kullanýcý silindiðinde tekrar ana sayfaya yönlendiriyoruz
        }

        // Kullanýcýyý düzenlemek için (GET)
        public async Task<IActionResult> Edit(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);  // Veritabanýndan kullanýcýyý alýyoruz
            if (user == null)
            {
                return NotFound();  // Kullanýcý bulunmazsa 404 döner
            }
            return View(user);  // Edit görünümüne yönlendirir
        }

        // Kullanýcýyý güncelleme iþlemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(user);  // Veritabanýnda kullanýcýyý güncelliyoruz
                return RedirectToAction("Index");  // Güncelleme sonrasý Index'e yönlendiriyoruz
            }
            return View(user);  // Hata varsa, kullanýcýyý ayný sayfada gösterir
        }



        // Güncellenmiþ kullanýcýyý kaydetme iþlemi
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(user);  // Kullanýcýyý güncelliyoruz
                return RedirectToAction("Index");  // Anasayfaya yönlendirme
            }
            return View(user);  // Eðer model geçerli deðilse, ayný view'i tekrar döndürüyoruz
        }
    }
}
