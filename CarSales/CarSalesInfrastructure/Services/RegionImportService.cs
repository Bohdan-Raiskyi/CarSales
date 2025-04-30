using ClosedXML.Excel;
using CarSalesDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CarSalesInfrastructure.Services
{
    public class RegionImportService : IImportService<Region>
    {
        private readonly CarSalesContext _context;
        private readonly List<string> _importLog = new();

        public RegionImportService(CarSalesContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            _importLog.Clear();

            if (!stream.CanRead)
            {
                _importLog.Add("Файл не читається.");
                return;
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                var worksheet = workBook.Worksheet(1);
                if (worksheet == null)
                {
                    _importLog.Add("Файл не містить жодного листа");
                    return;
                }

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Пропустити заголовок
                {
                    await TryAddUserAsync(row, cancellationToken);
                }
            }

            int saved = await _context.SaveChangesAsync(cancellationToken);
            _importLog.Add($"Збережено {saved} нових регіонів.");
        }

        public List<string> GetImportLog() => _importLog;

        private async Task TryAddUserAsync(IXLRow row, CancellationToken cancellationToken)
        {
            try
            {
                string regName = GetValueOrDefault(row, 1);

                if (string.IsNullOrEmpty(regName))
                {
                    _importLog.Add("Рядок без назви регіону — пропущено");
                    return;
                }

                if (!ValidateUserData(regName))
                {
                    _importLog.Add($"Регіон \"{regName}\" не пройшов валідацію — пропущено");
                    return;
                }

                var existingRegion = await _context.Regions.FirstOrDefaultAsync(u => u.RegName == regName, cancellationToken);

                if (existingRegion != null)
                {
                    _importLog.Add($"Регіон \"{regName}\" вже існує — пропущено");
                    return;
                }

                Region region = new Region
                {
                    RegName = regName
                };

                _importLog.Add($"Додається регіон: {regName}");
                _context.Regions.Add(region);
            }
            catch (Exception ex)
            {
                _importLog.Add($"Помилка в рядку: {ex.Message}");
                return;
            }
        }

        private bool ValidateUserData(string regName)
        {
            return regName.Length <= 50;
        }

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