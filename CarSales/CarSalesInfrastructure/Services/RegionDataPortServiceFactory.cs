using CarSalesDomain.Model;
using CarSalesInfrastructure.Services;
using System;

namespace CarSalesInfrastructure.Services
{
    public class RegionDataPortServiceFactory : IDataPortServiceFactory<Region>
    {
        private readonly CarSalesContext _context;

        public RegionDataPortServiceFactory(CarSalesContext context)
        {
            _context = context;
        }

        public IImportService<Region> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new RegionImportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано імпорт оголошень для типу контенту {contentType}");
        }

        public IExportService<Region> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new RegionExportService(_context);
            }

            throw new NotImplementedException($"Не реалізовано експорт оголошень для типу контенту {contentType}");
        }
    }
}
