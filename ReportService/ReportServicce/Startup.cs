using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportService.Common;
using ReportService.EmployeeCodeApi;
using ReportService.EmployeeSalaryApi;
using ReportService.ReportService;
using ReportService.Storage;

namespace ReportServicceNew
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEmployeeCodeApi>(x =>
                new EmployeeCodeApi(Settings.EmployeeCodeUrl, x.GetRequiredService<ILoggerFactory>()));
            services.AddTransient<IEmployeeSalaryApi>(x =>
                new EmployeeSalaryApi(Settings.EmployeeSalaryUrl, x.GetRequiredService<ILoggerFactory>()));
            services.AddTransient<IStorage>(x => new Storage(Settings.NpgsqlConnection));
            services.AddTransient<IReportBuilder, ReportBuilder>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}