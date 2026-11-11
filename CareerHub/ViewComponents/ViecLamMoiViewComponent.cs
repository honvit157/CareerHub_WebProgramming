
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.ViewComponents
{
    public class ViecLamMoiViewComponent : ViewComponent
    {
        private readonly CareerHubContext _context;

        public ViecLamMoiViewComponent(CareerHubContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jobs = await _context.Tintuyendungs
                .AsNoTracking()
                .Include(t => t.MaCongTyNavigation)
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Take(6)
                .ToListAsync();

            return View(jobs);
        }
    }
}
