using System;
using System.Collections.Generic;
using System.Linq;
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

            //return View(priceRange);
            return RedirectToAction("Index", "Ads", new { id = priceRange.Id, name = priceRange.RangeLabel });
        }

        // GET: PriceRanges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PriceRanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RangeLabel,Id")] PriceRange priceRange)
        {
            if (ModelState.IsValid)
            {
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RangeLabel,Id")] PriceRange priceRange)
        {
            if (id != priceRange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                _context.PriceRanges.Remove(priceRange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceRangeExists(int id)
        {
            return _context.PriceRanges.Any(e => e.Id == id);
        }
    }
}
