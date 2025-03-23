using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;
using CarSalesInfrastructure.Services;

namespace LibraryInfrastructure.Services
{
    public class AdExportService : IExportService<CarSalesDomain.Model.Ad>
    {
        private const string RootWorksheetName = "Всі оголошення";
        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Назва",
                "Тип авто",
                "Марка авто",
                "Ціна",
                "Ціновий діапазон",
                "Регіон",
                "Користувач",
                "Дата створення",
                "Дата продажу",
                "Опис"
            };

        private readonly DbContext _context;

        public AdExportService(DbContext context)
        {
            _context = context;
        }

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteAd(IXLWorksheet worksheet, CarSalesDomain.Model.Ad ad, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Type?.CarType1 ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Brand?.BrandName ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Price;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.PriceRange?.RangeLabel ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Region?.RegName ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.User?.UserName ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.CreationDate;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.SoldDate;
            worksheet.Cell(rowIndex, columnIndex++).Value = ad.Description;
        }

        private void WriteAds(IXLWorksheet worksheet, ICollection<CarSalesDomain.Model.Ad> ads)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var ad in ads)
            {
                WriteAd(worksheet, ad, rowIndex);
                rowIndex++;
            }
        }

        private void WriteByCarType(XLWorkbook workbook, IEnumerable<IGrouping<CarType, CarSalesDomain.Model.Ad>> adsByType)
        {
            foreach (var group in adsByType)
            {
                if (group.Key is not null)
                {
                    var worksheet = workbook.Worksheets.Add(group.Key.Id);
                    WriteAds(worksheet, group.ToList());
                }
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }

            var ads = await _context.Set<CarSalesDomain.Model.Ad>()
                .Include(ad => ad.Type)
                .Include(ad => ad.Brand)
                .Include(ad => ad.PriceRange)
                .Include(ad => ad.Region)
                .Include(ad => ad.User)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            // Створюємо загальний лист з усіма оголошеннями
            var allAdsWorksheet = workbook.Worksheets.Add(RootWorksheetName);
            WriteAds(allAdsWorksheet, ads);

            // Групуємо оголошення за типом авто
            var adsByType = ads.GroupBy(ad => ad.Type).Where(g => g.Key != null);
            WriteByCarType(workbook, adsByType);

            workbook.SaveAs(stream);
        }
    }
}