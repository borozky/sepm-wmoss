using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Data;
using WMoSS.Entities;

namespace WMoSS.ViewComponents
{
    [ViewComponent]
    public class TheatersListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public TheatersListViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke()
        {
            var theaters = db.Theaters.ToList();
            return View(theaters);
        }
    }
}
