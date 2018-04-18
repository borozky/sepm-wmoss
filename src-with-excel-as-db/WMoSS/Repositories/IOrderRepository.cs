using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface IOrderRepository
    {
        Order FindById(int orderId);
        IEnumerable<Order> FindAll();
        IEnumerable<Order> SearchByKeyword(string keyword);

        void Create(Order order);

        bool Exists(int orderId);
    }
}
