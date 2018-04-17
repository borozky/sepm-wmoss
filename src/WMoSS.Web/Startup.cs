using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using WMoSS.Repositories;
using WMoSS.Repositories.Excel;
using System.IO;
using WMoSS.Services;

namespace WMoSS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(serviceCollection =>
            {
                var rootPath = Env.ContentRootPath; 
                var excelFilePath = Path.Combine(rootPath, @"..\", @"WMOSS-EXCELDB.xlsx");

                if (Env.IsEnvironment("Testing"))
                {
                    var excelSamplePath = Path.Combine(rootPath, @"..\", @"WMOSS-EXCELDB-SAMPLE.xlsx");
                    var excelTestPath = Path.Combine(rootPath, @"..\", @"WMOSS-EXCELDB-TEST.xlsx");
                    File.Copy(excelFilePath, excelTestPath, true);
                }
                var fileInfo = new FileInfo(excelFilePath);
                if (fileInfo.Exists == false)
                {
                    throw new Exception("Excel file doesn't exists");
                }

                return new ExcelPackage(fileInfo);
            });
            services.AddTransient<IMovieRepository, MovieExcelRepository>();
            services.AddTransient<IMovieSessionRepository, MovieSessionExcelRepository>();
            services.AddTransient<ITheaterRepository, TheaterExcelRepository>();
            services.AddTransient<IOrderRepository, OrderExcelRepository>();
            services.AddTransient<ITicketRepository, TicketExcelRepository>();
            services.AddTransient<IUserRepository, UserExcelRepository>();
            services.AddTransient<IExpressBookingDataProvider, ExpressBookingDataProvider>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
