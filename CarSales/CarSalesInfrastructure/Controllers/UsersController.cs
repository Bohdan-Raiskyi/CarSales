using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;
using CarSalesInfrastructure;
using System.Text.RegularExpressions;
using CarSalesInfrastructure.Services;

namespace CarSalesInfrastructure.Controllers
{
    public class UsersController : Controller
    {
        private readonly CarSalesContext _context;
        private readonly UserDataPortServiceFactory _userDataPortServiceFactory;

        public UsersController(CarSalesContext context, UserDataPortServiceFactory userDataPortServiceFactory)
        {
            _context = context;
            _userDataPortServiceFactory = userDataPortServiceFactory;
        }

        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        CancellationToken cancellationToken = default)
        {
            var exportService = _userDataPortServiceFactory.GetExportService(contentType);
            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"users_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                ModelState.AddModelError("", "Файл не вибрано або він порожній.");
                return View();
            }

            try
            {
                var importService = _userDataPortServiceFactory.GetImportService(fileExcel.ContentType);
                using var stream = fileExcel.OpenReadStream();
                await importService.ImportFromStreamAsync(stream, cancellationToken);

                TempData["SuccessMessage"] = "Користувачів успішно імпортовано";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Помилка під час імпорту: {ex.Message}");
                return View();
            }
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            //return View(user);
            return RedirectToAction("Index", "SavedAds", new { userId = user.Id, userName = user.UserName });
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Password,PhoneNumber,CreatedDate,Id")] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Користувач з такою електронною поштою вже існує.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // Хешуємо пароль перед збереженням
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = string.Empty;

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserName,Email,Password,PhoneNumber,CreatedDate,Id")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            // Отримуємо існуючого користувача з БД
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Явно видаляємо валідацію паролю
            ModelState.Remove("Password");

            // Перевіряємо валідність моделі без врахування пароля
            bool isModelValid = ModelState.IsValid;

            // Встановлюємо правильну дату створення з існуючого запису
            user.CreatedDate = existingUser.CreatedDate;

            //user.Email = existingUser.Email;

            // Якщо поле пароля пусте — залишаємо старий хешований пароль
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = existingUser.Password;
            }
            else
            {
                // Перевіряємо новий пароль на відповідність вимогам вручну
                var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$";
                if (user.Password.Length < 8 || user.Password.Length > 20 ||
                    !System.Text.RegularExpressions.Regex.IsMatch(user.Password, passwordPattern))
                {
                    isModelValid = false;
                    ModelState.AddModelError("Password", "Пароль має містити від 8 до 20 символів, хоча б одну велику літеру, одну малу, одну цифру та один спеціальний символ.");
                }
                else
                {
                    // Хешуємо пароль, якщо він відповідає вимогам
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
            }

            if (isModelValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(user);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("UserName,Email,Password,PhoneNumber,CreatedDate,Id")] User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    // Отримуємо існуючого користувача з БД
        //    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        //    if (existingUser == null)
        //    {
        //        return NotFound();
        //    }


        //    // Видаляємо валідацію паролю з ModelState при редагуванні
        //    ModelState.Remove("Password");

        //    // Якщо поле пароля пусте — залишаємо старий хешований пароль
        //    if (string.IsNullOrWhiteSpace(user.Password))
        //    {
        //        user.Password = existingUser.Password;
        //    }
        //    else
        //    {
        //        // Перевіряємо новий пароль на відповідність вимогам вручну
        //        if (user.Password.Length < 8 || user.Password.Length > 20 ||
        //            !Regex.IsMatch(user.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$"))
        //        {
        //            ModelState.AddModelError("Password", "Пароль має містити від 8 до 20 символів, хоча б одну велику літеру, одну малу, одну цифру та один спеціальний символ.");
        //            return View(user);
        //        }

        //        // Якщо пароль відповідає вимогам - хешуємо його
        //        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        //    }
        //    //// Якщо поле пароля пусте — залишаємо старий хешований пароль
        //    //if (string.IsNullOrWhiteSpace(user.Password))
        //    //{
        //    //    user.Password = existingUser.Password;
        //    //}
        //    //else
        //    //{
        //    //    // Інакше хешуємо новий пароль
        //    //    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        //    //}


        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(user);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserExists(user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Typically means a foreign key constraint or other DB error
                TempData["DeleteError"] = "Неможливо видалити користувача, оскільки існують пов'язані записи.";
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}