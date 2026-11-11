
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace CareerHub.ViewComponents
{
    public class DanhMucViewComponent : ViewComponent
    {
        private readonly CareerHubContext _context;

        public DanhMucViewComponent(CareerHubContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucs = await _context.Danhmucs
                .AsNoTracking()
                .OrderBy(d => d.TenDanhMuc)
                .ToListAsync();
            

            return View(danhMucs);
        }
    }
}
