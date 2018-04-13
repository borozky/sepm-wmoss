using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> FindAll();
        ApplicationUser FindById(int userId);
        ApplicationUser FindByEmail(string email);
        ApplicationUser FindByEmailAndPassword(string email, string passwordHashed);

        void Create(ApplicationUser user);
        void Update(int userId, ApplicationUser user);
        bool Exists(int userId);
    }
}
