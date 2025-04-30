using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;

namespace CarSalesInfrastructure.Services
{
    public class RegionExportService : IExportService<CarSalesDomain.Model.Region>
    {
        private const string RootWorksheetName = "Всі користувачі";
        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Назва регіону"
            };

        private readonly DbContext _context;

        public RegionExportService(DbContext context)
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

        private void WriteRegion(IXLWorksheet worksheet, CarSalesDomain.Model.Region region, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = region.RegName;
        }

        private void WriteRegions(IXLWorksheet worksheet, ICollection<CarSalesDomain.Model.Region> regions)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var region in regions)
            {
                WriteRegion(worksheet, region, rowIndex);
                rowIndex++;
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }

            var regions = await _context.Set<CarSalesDomain.Model.Region>()
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            // Створюємо загальний лист з усіма користувачами
            var allRegionsWorksheet = workbook.Worksheets.Add(RootWorksheetName);
            WriteRegions(allRegionsWorksheet, regions);


            workbook.SaveAs(stream);
        }
    }
}