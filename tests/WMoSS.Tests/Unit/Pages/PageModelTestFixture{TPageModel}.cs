using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Data;

namespace WMoSS.Tests.Unit.Pages
{
    public abstract class PageModelTestFixture<TPageModel> : IDisposable where TPageModel : PageModel
    {
        protected DbContextOptions<ApplicationDbContext> dbContextOptions;
        protected Mock<ApplicationDbContext> mockDbContext;
        protected ApplicationDbContext db;
        protected Mock<ISession> mockSession;
        protected ISession session;
        protected HttpContext httpContext;
        protected ActionContext actionContext;
        protected IModelMetadataProvider modelMetadataProvider;
        protected ModelStateDictionary modelState;
        protected ViewDataDictionary viewData;
        protected TempDataDictionary tempData;
        protected PageContext pageContext;

        protected PageModelTestFixture()
        {
            dbContextOptions = Utilities.TestingDbContextOptions<ApplicationDbContext>();
            mockDbContext = new Mock<ApplicationDbContext>(dbContextOptions);
            db = mockDbContext.Object;
            mockSession = new Mock<ISession>();
            session = mockSession.Object;
            httpContext = new DefaultHttpContext
            {
                Session = session
            };
            modelState = new ModelStateDictionary();
            actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
            modelMetadataProvider = new EmptyModelMetadataProvider();
            viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
