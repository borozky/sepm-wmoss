using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface ITicketRepository
    {
        Ticket FindById(int ticketId);
        IEnumerable<Ticket> FindAll();
        IEnumerable<Ticket> FindAllByOrderId(int orderId);
        IEnumerable<Ticket> FIndAllByMovieSessionId(int movieSessionId);

        void Create(Ticket ticket);
        void Update(int ticketId, Ticket ticket);

        bool Exists(int ticketId);


    }
}
