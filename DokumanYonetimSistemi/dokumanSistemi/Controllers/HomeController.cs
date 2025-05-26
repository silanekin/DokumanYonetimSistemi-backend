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

        // Kullan�c�lar� listeleme i�lemi
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();  // Kullan�c�lar� �ekiyoruz
            return View(users);
        }
        // Yeni kullan�c� eklemek i�in
        public IActionResult Create()
        {
            return View();
        }

        // Kullan�c�y� eklemek i�in POST i�lemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.AddAsync(user);  // Yeni kullan�c�y� ekliyoruz
                return RedirectToAction("Index");
            }
            return View(user); // Hata varsa ayn� sayfada g�ster
        }

        // Kullan�c�y� silme i�lemi
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);  // Kullan�c�y� getiriyoruz
            if (user == null)
            {
                return NotFound();  // Kullan�c� bulunmazsa hata d�ner
            }

            await _userRepository.DeleteAsync(userId);  // Kullan�c�y� silme i�lemi
            return RedirectToAction("Index");  // Kullan�c� silindi�inde tekrar ana sayfaya y�nlendiriyoruz
        }

        // Kullan�c�y� d�zenlemek i�in (GET)
        public async Task<IActionResult> Edit(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);  // Veritaban�ndan kullan�c�y� al�yoruz
            if (user == null)
            {
                return NotFound();  // Kullan�c� bulunmazsa 404 d�ner
            }
            return View(user);  // Edit g�r�n�m�ne y�nlendirir
        }

        // Kullan�c�y� g�ncelleme i�lemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(user);  // Veritaban�nda kullan�c�y� g�ncelliyoruz
                return RedirectToAction("Index");  // G�ncelleme sonras� Index'e y�nlendiriyoruz
            }
            return View(user);  // Hata varsa, kullan�c�y� ayn� sayfada g�sterir
        }



        // G�ncellenmi� kullan�c�y� kaydetme i�lemi
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(user);  // Kullan�c�y� g�ncelliyoruz
                return RedirectToAction("Index");  // Anasayfaya y�nlendirme
            }
            return View(user);  // E�er model ge�erli de�ilse, ayn� view'i tekrar d�nd�r�yoruz
        }
    }
}
