using ClosedXML.Excel;
using CarSalesDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace CarSalesInfrastructure.Services
{
    public class AdImportService : IImportService<Ad>
    {
        private readonly CarSalesContext _context;

        public AdImportService(CarSalesContext context)
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
                // Перегляд усіх листів (в даному випадку типів авто)
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    // worksheet.Name - назва типу авто. Пробуємо знайти в БД, якщо відсутня, то пропускаємо лист
                    var typeName = worksheet.Name;
                    var carType = await _context.CarTypes.FirstOrDefaultAsync(type => type.CarType1 == typeName, cancellationToken);
                    if (carType == null)
                    {
                        // Якщо типу немає в базі, пропускаємо цей лист
                        continue;
                    }

                    // Перегляд усіх рядків                    
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    // Пропустити перший рядок, бо це заголовок
                    {
                        await TryAddAdAsync(row, cancellationToken, carType);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task TryAddAdAsync(IXLRow row, CancellationToken cancellationToken, CarType carType)
        {
            try
            {
                // Перевірка наявності всіх обов'язкових даних
                if (string.IsNullOrEmpty(GetValueOrDefault(row, 1)) ||
                    !TryGetIntValue(row, 3, out int price))
                {
                    // Якщо назва відсутня або ціна невалідна, пропускаємо запис
                    return;
                }

                // Перевірка довжини назви
                string name = GetValueOrDefault(row, 1);
                if (name.Length > 50)
                {
                    // Якщо назва задовга, пропускаємо запис
                    return;
                }

                // Перевірка довжини опису
                string description = GetValueOrDefault(row, 2);
                if (description.Length > 4000)
                {
                    // Якщо опис задовгий, пропускаємо запис
                    return;
                }

                // Перевірка брендів, регіонів та користувачів
                var brandName = GetValueOrDefault(row, 4);
                var regionName = GetValueOrDefault(row, 5);
                var userName = GetValueOrDefault(row, 6);

                var brand = await _context.CarBrands.FirstOrDefaultAsync(b => b.BrandName == brandName, cancellationToken);
                var region = await _context.Regions.FirstOrDefaultAsync(r => r.RegName == regionName, cancellationToken);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

                // Перевірка наявності брендів, регіонів та користувачів
                if (brand == null || region == null || user == null)
                {
                    return;
                }

                // Отримуємо значення PriceRangeId
                bool hasPriceRangeId = TryGetIntValue(row, 7, out int priceRangeId);
                if (!hasPriceRangeId)
                {
                    // Якщо значення Id цінового діапазону не вказане або невалідне, пропускаємо запис
                    return;
                }

                // Створюємо новий запис оголошення
                Ad ad = new Ad();
                ad.Name = name;
                ad.Description = description;
                ad.Price = price;
                ad.CreationDate = DateTime.Now;
                ad.TypeId = carType.Id;
                ad.Type = carType;
                ad.BrandId = brand.Id;
                ad.Brand = brand;
                ad.RegionId = region.Id;
                ad.Region = region;
                ad.UserId = user.Id;
                ad.User = user;
                ad.PriceRangeId = priceRangeId;

                _context.Ads.Add(ad);
            }
            catch
            {
                // Якщо сталася помилка при обробці рядка, просто пропускаємо його
                return;
            }
        }

        // Допоміжний метод для безпечного отримання значення з комірки
        private static string GetValueOrDefault(IXLRow row, int cellIndex)
        {
            try
            {
                return row.Cell(cellIndex).Value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        // Допоміжний метод для безпечного конвертування значення в int
        private static bool TryGetIntValue(IXLRow row, int cellIndex, out int value)
        {
            value = 0;
            try
            {
                string stringValue = row.Cell(cellIndex).Value.ToString();
                return int.TryParse(stringValue, out value) && value >= 0;
            }
            catch
            {
                return false;
            }
        }
    }
}