using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;
using CarSalesInfrastructure;

namespace CarSalesInfrastructure.Controllers
{
    public class ImagesController : Controller
    {
        private readonly CarSalesContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagesController(CarSalesContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            var carSalesContext = _context.Images.Include(i => i.Ad);
            return View(await carSalesContext.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Ad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name");
            return View();
        }

        // POST: Images/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int AdId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Будь ласка, оберіть файл зображення для завантаження.";
                ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", AdId);
                return View();
            }

            // Перевірка типу файлу (дозволені лише зображення)
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Дозволені тільки файли зображень (JPG, JPEG, PNG, GIF, BMP).";
                ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", AdId);
                return View();
            }

            // Створюємо підпапку для оголошення, якщо вона не існує
            string folderName = $"ads{AdId}";
            string webRootPath = _webHostEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "images", folderName);

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Генеруємо унікальне ім'я файлу
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string fullPath = Path.Combine(newPath, fileName);

            try
            {
                // Зберігаємо файл
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Зберігаємо шлях у базі даних
                string dbPath = Path.Combine("images", folderName, fileName).Replace("\\", "/");

                Image image = new Image
                {
                    AdId = AdId,
                    Path = dbPath
                };

                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при завантаженні файлу: {ex.Message}";
                ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", AdId);
                return View();
            }
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", image.AdId);
            return View(image);
        }

        // POST: Images/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int AdId, IFormFile imageFile)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                // Видаляємо старий файл, якщо він існує
                if (!string.IsNullOrEmpty(image.Path))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Path);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Створюємо підпапку для оголошення, якщо вона не існує
                string folderName = $"ads{AdId}";
                string webRootPath = _webHostEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, "images", folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                // Генеруємо унікальне ім'я файлу
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string fullPath = Path.Combine(newPath, fileName);

                // Зберігаємо файл
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Оновлюємо шлях у моделі
                image.Path = Path.Combine("images", folderName, fileName).Replace("\\", "/");
            }

            image.AdId = AdId;

            try
            {
                _context.Update(image);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(image.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Ad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                // Видаляємо файл
                if (!string.IsNullOrEmpty(image.Path))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Path);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Images.Remove(image);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Типова помилка зв'язків або інша помилка бази даних
                TempData["DeleteError"] = "Неможливо видалити зображення, оскільки існують пов'язані записи.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }
    }
}