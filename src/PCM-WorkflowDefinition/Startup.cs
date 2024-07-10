using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Infrastructure;
using Infrastructure.Workflow.Activities;
using Infrastructure.Workflow.Scripting.JavaScript;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using AutoMapper;
using AutoMapper.Internal;

namespace PCM_WorkflowDefinition
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();
            services
                .AddDbContext(Configuration)
                // Servizi di infrastruttura
                .AddInfrastructureServices(Configuration)
                // Servizi di identity
                .AddIdentity(Configuration);

            var elsaSection = Configuration.GetSection("Elsa");

            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddEmailActivities(Configuration.GetSection("MailSettings").Bind)
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddActivitiesFrom<GetAccordoActivity>()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)

                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // For Dashboard.
            services.AddRazorPages();

            // Automapper
            services.AddAutoMapper(cfg => cfg.ShouldMapMethod = (cfg => false), typeof(Mapper).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseStaticFiles() // For Dashboard.
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                });
        }
    }
}
