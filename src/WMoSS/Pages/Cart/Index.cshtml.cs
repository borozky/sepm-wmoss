using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;

namespace WMoSS.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public CartItem CartItem { get; set; }

        public IActionResult OnPost()
        {
            return new JsonResult(CartItem);
        }

    }
}