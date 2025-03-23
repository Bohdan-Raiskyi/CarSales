using CarSalesDomain.Model;
using LibraryInfrastructure.Services;
using System;

namespace CarSalesInfrastructure.Services
{
    public class AdDataPortServiceFactory : IDataPortServiceFactory<Ad>
    {
        private readonly CarSalesContext _context;

        public AdDataPortServiceFactory(CarSalesContext context)
        {
            _context = context;
        }

        public IImportService<Ad> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new AdImportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано імпорт оголошень для типу контенту {contentType}");
        }

        public IExportService<Ad> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new AdExportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано експорт оголошень для типу контенту {contentType}");
        }
    }
}
