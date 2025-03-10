﻿using System;
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
    public class AdsController : Controller
    {
        private readonly CarSalesContext _context;

        public AdsController(CarSalesContext context)
        {
            _context = context;
        }

        // GET: Ads
        public async Task<IActionResult> Index()
        {
            var carSalesContext = _context.Ads
                .Include(a => a.Brand)
                .Include(a => a.PriceRange)
                .Include(a => a.Region)
                .Include(a => a.Type)
                .Include(a => a.User)
                .OrderBy(a => a.SoldDate == null ? 0 : 1) // Активні спочатку, продані в кінці
                .ThenByDescending(a => a.CreationDate);
            return View(await carSalesContext.ToListAsync());
        }
        //public async Task<ActionResult> Index(int? id, string? name)
        //{
        //    if (id == null) return RedirectToAction("PriceRanges", "Index");

        //    // Закріплення книг за категорією
        //    ViewBag.PriceRangeId = id;
        //    ViewBag.RangeLabel = name;

        //    var booksByCategory = await _context.Ads
        //        .Where(b => b.PriceRangeId == id)
        //        .Include(b => b.PriceRange)
        //        .ToListAsync();

        //    return View(booksByCategory);
        //}


        // GET: Ads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .Include(a => a.Brand)
                .Include(a => a.PriceRange)
                .Include(a => a.Region)
                .Include(a => a.Type)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            //return View(ad);
            return RedirectToAction("Index", "Images", new { id = ad.Id, name = ad.Name });
        }

        // GET: Ads/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "BrandName");
            ViewData["PriceRangeId"] = new SelectList(_context.PriceRanges, "Id", "RangeLabel");
            ViewData["RegionId"] = new SelectList(_context.Regions, "Id", "RegName");
            ViewData["TypeId"] = new SelectList(_context.CarTypes, "Id", "CarType1");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Ads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,TypeId,BrandId,PriceRangeId,RegionId,Price,Name,Description,CreationDate,SoldDate,Id")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "BrandName", ad.BrandId);
            ViewData["PriceRangeId"] = new SelectList(_context.PriceRanges, "Id", "RangeLabel", ad.PriceRangeId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "Id", "RegName", ad.RegionId);
            ViewData["TypeId"] = new SelectList(_context.CarTypes, "Id", "CarType1", ad.TypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", ad.UserId);
            return View(ad);
        }

        // GET: Ads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "BrandName", ad.BrandId);
            ViewData["PriceRangeId"] = new SelectList(_context.PriceRanges, "Id", "RangeLabel", ad.PriceRangeId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "Id", "RegName", ad.RegionId);
            ViewData["TypeId"] = new SelectList(_context.CarTypes, "Id", "CarType1", ad.TypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", ad.UserId);
            return View(ad);
        }

        // POST: Ads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,TypeId,BrandId,PriceRangeId,RegionId,Price,Name,Description,CreationDate,SoldDate,Id")] Ad ad)
        {
            if (id != ad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.Id))
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
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "BrandName", ad.BrandId);
            ViewData["PriceRangeId"] = new SelectList(_context.PriceRanges, "Id", "RangeLabel", ad.PriceRangeId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "Id", "RegName", ad.RegionId);
            ViewData["TypeId"] = new SelectList(_context.CarTypes, "Id", "CarType1", ad.TypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", ad.UserId);
            return View(ad);
        }

        public IActionResult Sell(int id)
        {
            var ad = _context.Ads
                .Include(a => a.Brand)
                .Include(a => a.PriceRange)
                .Include(a => a.Region)
                .Include(a => a.Type)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        [HttpPost]
        public IActionResult SellConfirmed(int id)
        {
            var ad = _context.Ads.Find(id);
            if (ad == null)
            {
                return NotFound();
            }

            ad.SoldDate = DateTime.Now; // Встановлюємо дату продажу
            _context.SaveChanges();

            return RedirectToAction("Index", "Ads");
        }



        // GET: Ads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .Include(a => a.Brand)
                .Include(a => a.PriceRange)
                .Include(a => a.Region)
                .Include(a => a.Type)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad != null)
            {
                _context.Ads.Remove(ad);
            }

            try
            {
                _context.Ads.Remove(ad);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Typically means a foreign key constraint or other DB error
                TempData["DeleteError"] = "Неможливо видалити оголошення, оскільки існують пов'язані записи.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
            return _context.Ads.Any(e => e.Id == id);
        }
    }
}
