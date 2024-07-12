using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Savorboard.CAP.InMemoryMessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_WorkflowDashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // Registra framework CAP per monitoraggio
            services.AddCap(x =>
            {
                // Sql server come sistema di storage.
                x.UseSqlServer(Configuration.GetConnectionString("CAPConnection"));

                // Memoria come trasporto.
                x.UseInMemoryMessageQueue();

                // Dashboard di monitoraggio
                x.UseDashboard();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
