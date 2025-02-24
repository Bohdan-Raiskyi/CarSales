using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSalesInfrastructure;
using CarSalesDomain.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarSalesInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly CarSalesContext _context;

        public record CountByYearResponseItem(string Year, int Count);
        public record PieDataResponseItem(string Label, int Value);

        public ChartController(CarSalesContext context)
        {
            _context = context;
        }


        [HttpGet("ads-by-price-range")]
        public async Task<ActionResult<IEnumerable<PieDataResponseItem>>> GetAdsByPriceRange()
        {
            var data = await _context.Ads
                .GroupBy(a => a.PriceRangeId)
                .Select(g => new
                {
                    PriceRangeName = g.Select(a => a.PriceRange.RangeLabel).FirstOrDefault() ?? "Невідомо",
                    Count = g.Count()
                })
                .ToListAsync();

            var result = data.Select(d => new PieDataResponseItem(d.PriceRangeName, d.Count)).ToList();
            return Ok(result);
        }


        [HttpGet("ads-by-region")]
        public async Task<ActionResult<IEnumerable<PieDataResponseItem>>> GetAdsByRegion()
        {
            var data = await _context.Ads
                .GroupBy(a => a.RegionId)
                .Select(g => new
                {
                    RegionName = g.Select(a => a.Region.RegName).FirstOrDefault() ?? "Невідомо",
                    Count = g.Count()
                })
                .ToListAsync();

            var result = data.Select(d => new PieDataResponseItem(d.RegionName, d.Count)).ToList();
            return Ok(result);
        }
    }
}