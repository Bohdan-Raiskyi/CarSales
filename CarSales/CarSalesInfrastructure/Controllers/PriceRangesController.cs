using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;
using CarSalesInfrastructure;

namespace CarSalesInfrastructure.Controllers
{
    public class PriceRangesController : Controller
    {
        private readonly CarSalesContext _context;

        public PriceRangesController(CarSalesContext context)
        {
            _context = context;
        }

        // GET: PriceRanges
        public async Task<IActionResult> Index()
        {
            return View(await _context.PriceRanges.ToListAsync());
        }

        // GET: PriceRanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceRange == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Ads", new { id = priceRange.Id, name = priceRange.RangeLabel });
        }

        // GET: PriceRanges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PriceRanges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RangeLabel,Id")] PriceRange priceRange)
        {
            // Перевірка формату
            if (!IsValidPriceRangeFormat(priceRange.RangeLabel))
            {
                ModelState.AddModelError("RangeLabel",
                    "Невірний формат діапазону. Використовуйте один з форматів: >N, <N або N-M, де N і M - числа, а N < M");
                return View(priceRange);
            }

            if (ModelState.IsValid)
            {
                // Перевірка на перетин з існуючими діапазонами
                var existingRanges = await _context.PriceRanges.ToListAsync();
                (decimal? newMin, decimal? newMax) = ParsePriceRange(priceRange.RangeLabel);

                // Перевіряємо перетин тільки якщо вдалося розпарсити діапазон
                if (newMin.HasValue && newMax.HasValue)
                {
                    foreach (var existingRange in existingRanges)
                    {
                        (decimal? existingMin, decimal? existingMax) = ParsePriceRange(existingRange.RangeLabel);

                        // Перевіряємо перетин тільки якщо вдалося розпарсити існуючий діапазон
                        if (existingMin.HasValue && existingMax.HasValue)
                        {
                            // Перевірка на перетин діапазонів
                            if (DoRangesOverlap(newMin.Value, newMax.Value, existingMin.Value, existingMax.Value))
                            {
                                ModelState.AddModelError("RangeLabel",
                                    $"Цей діапазон перетинається з існуючим діапазоном '{existingRange.RangeLabel}'");
                                return View(priceRange);
                            }
                        }
                    }
                }

                _context.Add(priceRange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priceRange);
        }

        // GET: PriceRanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges.FindAsync(id);
            if (priceRange == null)
            {
                return NotFound();
            }
            return View(priceRange);
        }

        // POST: PriceRanges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RangeLabel,Id")] PriceRange priceRange)
        {
            if (id != priceRange.Id)
            {
                return NotFound();
            }

            // Перевірка формату
            if (!IsValidPriceRangeFormat(priceRange.RangeLabel))
            {
                ModelState.AddModelError("RangeLabel",
                    "Невірний формат діапазону. Використовуйте один з форматів: >N, <N або N-M, де N і M - числа, а N < M");
                return View(priceRange);
            }

            if (ModelState.IsValid)
            {
                // Перевірка на перетин з існуючими діапазонами
                var existingRanges = await _context.PriceRanges
                    .Where(r => r.Id != id) // Виключаємо поточний діапазон
                    .ToListAsync();

                (decimal? newMin, decimal? newMax) = ParsePriceRange(priceRange.RangeLabel);

                // Перевіряємо перетин тільки якщо вдалося розпарсити діапазон
                if (newMin.HasValue && newMax.HasValue)
                {
                    foreach (var existingRange in existingRanges)
                    {
                        (decimal? existingMin, decimal? existingMax) = ParsePriceRange(existingRange.RangeLabel);

                        // Перевіряємо перетин тільки якщо вдалося розпарсити існуючий діапазон
                        if (existingMin.HasValue && existingMax.HasValue)
                        {
                            // Перевірка на перетин діапазонів
                            if (DoRangesOverlap(newMin.Value, newMax.Value, existingMin.Value, existingMax.Value))
                            {
                                ModelState.AddModelError("RangeLabel",
                                    $"Цей діапазон перетинається з існуючим діапазоном '{existingRange.RangeLabel}'");
                                return View(priceRange);
                            }
                        }
                    }
                }

                try
                {
                    _context.Update(priceRange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceRangeExists(priceRange.Id))
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
            return View(priceRange);
        }

        // Метод для перевірки формату цінового діапазону
        private bool IsValidPriceRangeFormat(string rangeLabel)
        {
            if (string.IsNullOrEmpty(rangeLabel))
                return false;

            // Перевірка формату ">N"
            if (rangeLabel.StartsWith(">"))
            {
                string valueStr = rangeLabel.Substring(1);
                if (!decimal.TryParse(valueStr, out decimal value))
                    return false;

                return value >= 0;
            }

            // Перевірка формату "<N"
            if (rangeLabel.StartsWith("<"))
            {
                string valueStr = rangeLabel.Substring(1);
                if (!decimal.TryParse(valueStr, out decimal value))
                    return false;

                return value > 0;
            }

            // Перевірка формату "N-M"
            if (rangeLabel.Contains("-"))
            {
                string[] parts = rangeLabel.Split('-');
                if (parts.Length != 2)
                    return false;

                if (!decimal.TryParse(parts[0], out decimal minValue) ||
                    !decimal.TryParse(parts[1], out decimal maxValue))
                    return false;

                // Перевірка, що N < M і обидва невід'ємні
                return minValue < maxValue && minValue >= 0 && maxValue > 0;
            }

            // Якщо жоден формат не підходить
            return false;
        }

        // Метод для перевірки перетину двох діапазонів
        private bool DoRangesOverlap(decimal min1, decimal max1, decimal min2, decimal max2)
        {
            // Два діапазони перетинаються, якщо один з них починається до завершення іншого
            return min1 <= max2 && min2 <= max1;
        }

        // Метод для розпарсування рядка цінового діапазону
        private (decimal? min, decimal? max) ParsePriceRange(string rangeLabel)
        {
            if (string.IsNullOrEmpty(rangeLabel))
                return (null, null);

            // Варіант "<500" (менше ніж)
            if (rangeLabel.StartsWith("<"))
            {
                if (decimal.TryParse(rangeLabel.Substring(1), out decimal maxValue))
                {
                    return (decimal.Zero, maxValue);
                }
            }
            // Варіант ">50000" (більше ніж)
            else if (rangeLabel.StartsWith(">"))
            {
                if (decimal.TryParse(rangeLabel.Substring(1), out decimal minValue))
                {
                    return (minValue, decimal.MaxValue);
                }
            }
            // Варіант "1000-2000" (діапазон)
            else if (rangeLabel.Contains("-"))
            {
                string[] parts = rangeLabel.Split('-');
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out decimal minValue) &&
                    decimal.TryParse(parts[1], out decimal maxValue))
                {
                    return (minValue, maxValue);
                }
            }

            return (null, null);
        }

        // GET: PriceRanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceRange == null)
            {
                return NotFound();
            }

            return View(priceRange);
        }

        // POST: PriceRanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceRange = await _context.PriceRanges.FindAsync(id);
            if (priceRange != null)
            {
                try
                {
                    _context.PriceRanges.Remove(priceRange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Typically means a foreign key constraint or other DB error
                    TempData["DeleteError"] = "Неможливо видалити ціновий діапазон, оскільки існують пов'язані записи.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PriceRangeExists(int id)
        {
            return _context.PriceRanges.Any(e => e.Id == id);
        }
    }
}