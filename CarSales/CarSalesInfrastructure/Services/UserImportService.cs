using ClosedXML.Excel;
using CarSalesDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CarSalesInfrastructure.Services
{
    public class UserImportService : IImportService<User>
    {
        private readonly CarSalesContext _context;
        private static readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private static readonly Regex _phoneRegex = new Regex(@"^\d{10}$");
        private static readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$");

        public UserImportService(CarSalesContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                // Беремо перший лист для імпорту користувачів
                var worksheet = workBook.Worksheet(1);
                if (worksheet == null)
                {
                    throw new ArgumentException("Файл не містить жодного листа");
                }

                // Перегляд усіх рядків
                foreach (var row in worksheet.RowsUsed().Skip(1)) // Пропустити перший рядок, бо це заголовок
                {
                    await TryAddUserAsync(row, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task TryAddUserAsync(IXLRow row, CancellationToken cancellationToken)
        {
            try
            {
                // Отримання даних з рядка
                string userName = GetValueOrDefault(row, 1);
                string email = GetValueOrDefault(row, 2);
                string phoneNumber = GetValueOrDefault(row, 3);
                string createdDateStr = GetValueOrDefault(row, 4);
                string password = GetValueOrDefault(row, 5);

                // Перевірка наявності обов'язкових полів
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phoneNumber))
                {
                    return;
                }

                // Перевірка валідності даних
                if (!ValidateUserData(userName, email, phoneNumber, password, out DateTime createdDate, createdDateStr))
                {
                    return;
                }

                // Перевірка чи користувач уже існує
                var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == email || u.UserName == userName, cancellationToken);

                if (existingUser != null)
                {
                    return; // Користувач уже існує
                }

                // Створення нового користувача
                User user = new User
                {
                    UserName = userName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Password = password,
                    CreatedDate = createdDate
                };
                Console.WriteLine($"Додаю користувача: {userName}, {email}, {phoneNumber}, {password}");

                _context.Users.Add(user);
            }
            catch
            {
                // Якщо сталася помилка при обробці рядка, просто пропускаємо його
                return;
            }
        }

        // Валідація даних користувача
        private bool ValidateUserData(string userName, string email, string phoneNumber, string password,
            out DateTime createdDate, string createdDateStr)
        {
            createdDate = DateTime.Now;

            ////Перевірка імені користувача
            //if (userName.Length > 50)
            //{
            //    return false;
            //}

            ////Перевірка електронної пошти
            //if (email.Length > 100 || !_emailRegex.IsMatch(email))
            //{
            //    return false;
            //}

            ////Перевірка номера телефону
            //if (!_phoneRegex.IsMatch(phoneNumber))
            //{
            //    return false;
            //}

            ////Перевірка пароля
            //if (password.Length < 8 || password.Length > 20 || !_passwordRegex.IsMatch(password))
            //{
            //    return false;
            //}

            ////Парсинг дати
            //if (!DateTime.TryParse(createdDateStr, out createdDate))
            //{
            //    createdDate = DateTime.Now;
            //}

            return true;
        }

        // Допоміжний метод для безпечного отримання значення з комірки
        private static string GetValueOrDefault(IXLRow row, int cellIndex)
        {
            try
            {
                return row.Cell(cellIndex).Value.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}