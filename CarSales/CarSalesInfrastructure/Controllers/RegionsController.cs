using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;
using CarSalesInfrastructure;
using CarSalesInfrastructure.Services;
using ClosedXML.Excel;

namespace CarSalesInfrastructure.Controllers
{
    public class RegionsController : Controller
    {
        private readonly CarSalesContext _context;
        private readonly RegionDataPortServiceFactory _regionDataPortServiceFactory;

        public RegionsController(CarSalesContext context, RegionDataPortServiceFactory regionDataPortServiceFactory)
        {
            _context = context;
            _regionDataPortServiceFactory = regionDataPortServiceFactory;
        }

        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        CancellationToken cancellationToken = default)
        {
            var exportService = _regionDataPortServiceFactory.GetExportService(contentType);
            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"regions_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
        private async Task<bool> TrySaveRegionAsync(Region region, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(region.RegName) || region.RegName.Length > 50)
                return false;

            var exists = await _context.Regions.AnyAsync(r => r.RegName == region.RegName, cancellationToken);
            if (exists)
                return false;

            _context.Regions.Add(region);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                ModelState.AddModelError("", "Файл не вибрано або він порожній.");
                return View();
            }

            try
            {
                using var stream = fileExcel.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var regName = row.Cell(1).Value.ToString().Trim();
                    if (!string.IsNullOrEmpty(regName))
                    {
                        await TrySaveRegionAsync(new Region { RegName = regName }, cancellationToken);
                    }
                }

                TempData["SuccessMessage"] = "Регіони успішно імпортовано";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Помилка під час імпорту: {ex.Message}");
                return View();
            }
        }


        // GET: Regions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Regions.ToListAsync());
        }

        // GET: Regions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Regions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (region == null)
            {
                return NotFound();
            }

            //return View(region);
            return RedirectToAction("Index", "Ads", new { id = region.Id, name = region.RegName });
        }

        // GET: Regions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("RegName,Id")] Region region)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(region);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(region);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegName,Id")] Region region)
        {
            if (await TrySaveRegionAsync(region, CancellationToken.None))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Не вдалося створити регіон.");
            return View(region);
        }

        // GET: Regions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegName,Id")] Region region)
        {
            if (id != region.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(region);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegionExists(region.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(region);
        }

        // GET: Regions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Regions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region != null)
            {
                _context.Regions.Remove(region);
            }

            try
            {
                _context.Regions.Remove(region);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Typically means a foreign key constraint or other DB error
                TempData["DeleteError"] = "Неможливо видалити регіон, оскільки існують пов'язані записи.";
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegionExists(int id)
        {
            return _context.Regions.Any(e => e.Id == id);
        }
    }
}
