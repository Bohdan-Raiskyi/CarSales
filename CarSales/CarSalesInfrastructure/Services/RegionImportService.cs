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
    public class RegionImportService : IImportService<Region>
    {
        private readonly CarSalesContext _context;

        public RegionImportService(CarSalesContext context)
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
                string regName = GetValueOrDefault(row, 1);

                // Перевірка наявності обов'язкових полів
                if (string.IsNullOrEmpty(regName))
                {
                    return;
                }

                // Перевірка валідності даних
                if (!ValidateUserData(regName))
                {
                    return;
                }

                // Перевірка чи користувач уже існує
                var existingRegion = await _context.Regions.FirstOrDefaultAsync(u =>
                    u.RegName == regName, cancellationToken);

                if (existingRegion != null)
                {
                    return; // Користувач уже існує
                }

                // Створення нового користувача
                Region region = new Region
                {
                    RegName = regName
                };
                Console.WriteLine($"Додаю регіон: {regName}");

                _context.Regions.Add(region);
            }
            catch
            {
                // Якщо сталася помилка при обробці рядка, просто пропускаємо його
                return;
            }
        }

        // Валідація даних користувача
        private bool ValidateUserData(string regName)
        {
            //createdDate = DateTime.Now;

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