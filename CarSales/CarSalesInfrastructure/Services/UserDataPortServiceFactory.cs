using CarSalesDomain.Model;
using LibraryInfrastructure.Services;
using System;

namespace CarSalesInfrastructure.Services
{
    public class UserDataPortServiceFactory : IDataPortServiceFactory<User>
    {
        private readonly CarSalesContext _context;

        public UserDataPortServiceFactory(CarSalesContext context)
        {
            _context = context;
        }

        public IImportService<User> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new UserImportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано імпорт оголошень для типу контенту {contentType}");
        }

        public IExportService<User> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new UserExportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано експорт оголошень для типу контенту {contentType}");
        }
    }
}
