using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface ITheaterRepository
    {
        IEnumerable<Theater> FindAll();
    }
}
