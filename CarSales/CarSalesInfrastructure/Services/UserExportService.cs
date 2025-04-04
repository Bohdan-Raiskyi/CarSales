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
    public class UserExportService : IExportService<CarSalesDomain.Model.User>
    {
        private const string RootWorksheetName = "Всі користувачі";
        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Ім'я користувача",
                "Пошта",
                "Номер телефону",
                "Дата створення",
                "Пароль"
            };

        private readonly DbContext _context;

        public UserExportService(DbContext context)
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

        private void WriteUser(IXLWorksheet worksheet, CarSalesDomain.Model.User user, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = user.UserName;
            worksheet.Cell(rowIndex, columnIndex++).Value = user.Email;
            worksheet.Cell(rowIndex, columnIndex++).Value = user.PhoneNumber ?? string.Empty;
            worksheet.Cell(rowIndex, columnIndex++).Value = user.CreatedDate;
            worksheet.Cell(rowIndex, columnIndex++).Value = user.Password;
        }

        private void WriteUsers(IXLWorksheet worksheet, ICollection<CarSalesDomain.Model.User> users)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var user in users)
            {
                WriteUser(worksheet, user, rowIndex);
                rowIndex++;
            }
        }

        private void WriteByCreationYear(XLWorkbook workbook, IEnumerable<IGrouping<int, CarSalesDomain.Model.User>> usersByYear)
        {
            foreach (var group in usersByYear)
            {
                var worksheet = workbook.Worksheets.Add($"Користувачі {group.Key} року");
                WriteUsers(worksheet, group.ToList());
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }

            var users = await _context.Set<CarSalesDomain.Model.User>()
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            // Створюємо загальний лист з усіма користувачами
            var allUsersWorksheet = workbook.Worksheets.Add(RootWorksheetName);
            WriteUsers(allUsersWorksheet, users);

            // Групуємо користувачів за роком створення
            var usersByYear = users.GroupBy(user => user.CreatedDate.Year);
            WriteByCreationYear(workbook, usersByYear);

            workbook.SaveAs(stream);
        }
    }
}