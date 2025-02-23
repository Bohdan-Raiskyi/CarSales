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
    public class SavedAdsController : Controller
    {
        private readonly CarSalesContext _context;

        public SavedAdsController(CarSalesContext context)
        {
            _context = context;
        }

        // GET: SavedAds
        public async Task<IActionResult> Index()
        {
            var carSalesContext = _context.SavedAds.Include(s => s.Ad).Include(s => s.User);
            return View(await carSalesContext.ToListAsync());
        }

        // GET: SavedAds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedAd = await _context.SavedAds
                .Include(s => s.Ad)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedAd == null)
            {
                return NotFound();
            }

            return View(savedAd);
        }

        // GET: SavedAds/Create
        public IActionResult Create()
        {
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: SavedAds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,AdId,SavedDate,Id")] SavedAd savedAd)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savedAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", savedAd.AdId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", savedAd.UserId);
            return View(savedAd);
        }

        // GET: SavedAds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedAd = await _context.SavedAds.FindAsync(id);
            if (savedAd == null)
            {
                return NotFound();
            }
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", savedAd.AdId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", savedAd.UserId);
            return View(savedAd);
        }

        // POST: SavedAds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,AdId,SavedDate,Id")] SavedAd savedAd)
        {
            if (id != savedAd.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedAd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedAdExists(savedAd.Id))
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
            ViewData["AdId"] = new SelectList(_context.Ads, "Id", "Name", savedAd.AdId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", savedAd.UserId);
            return View(savedAd);
        }

        // GET: SavedAds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedAd = await _context.SavedAds
                .Include(s => s.Ad)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedAd == null)
            {
                return NotFound();
            }

            return View(savedAd);
        }

        // POST: SavedAds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savedAd = await _context.SavedAds.FindAsync(id);
            if (savedAd != null)
            {
                _context.SavedAds.Remove(savedAd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedAdExists(int id)
        {
            return _context.SavedAds.Any(e => e.Id == id);
        }
    }
}
