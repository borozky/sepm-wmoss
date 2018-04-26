using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using Xunit;

namespace WMoSS.Tests.Unit.Pages.Order
{
    public class OrderDetailsTests : PageModelTestFixture<WMoSS.Pages.Order.DetailsModel>
    {
        private WMoSS.Pages.Order.DetailsModel OrderDetailsModel;

        public OrderDetailsTests()
        {
            OrderDetailsModel = new WMoSS.Pages.Order.DetailsModel(db)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(pageContext)
            };
        }

        [Fact]
        public async Task TestOrderDetailsPage()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                DbInitializer.Initialize(db);

                OrderDetailsModel = new WMoSS.Pages.Order.DetailsModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };

                var order = new WMoSS.Entities.Order
                {
                    FullName = "Sample user",
                    MailingAddress = "123 Club house avenue",
                    EmailAddress = "someone123@email.com",
                    CardNumber = "4111111111111111",
                    Expiry = "10/20",
                    Tickets = new WMoSS.Entities.Ticket[]
                    {
                        new WMoSS.Entities.Ticket
                        {
                            MovieSessionId = 1,
                            SeatNumber = "A1"
                        }
                    }
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync();


                // ACT
                var result = await OrderDetailsModel.OnGet(order.Id);

                // ASSERT
                Assert.IsType<PageResult>(result);
                Assert.NotNull(OrderDetailsModel.OrderDetails);

                
            }
        }
    }
}
