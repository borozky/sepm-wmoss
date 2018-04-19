using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace WMoSS.Pages.Order
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Entities.Order Order { get; set; }

        public async Task<IActionResult> OnGet(int id, CancellationToken ct)
        {
            Order = await _context.Orders
                .Include(o => o.Tickets)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (Order == null)
            {
                return NotFound();
            }

            return Page();
                
        }
    }
}