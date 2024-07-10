using Elsa;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models.AutoMapper;
using ReflectionIT.Mvc.Paging;
using System.Globalization;

namespace PCM_LavoroAgile
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            //Paginazione
            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap4";
                options.HtmlIndicatorDown = " <span>&darr;</span>";
                options.HtmlIndicatorUp = " <span>&uarr;</span>";
            });

            // Automapper
            services.AddAutoMapper(cfg => cfg.ShouldMapField = (cfg => false), typeof(LavoroAgileMapperProfile));

            services
                // Configurazione
                //.AddSingleton(Configuration.Get<AppSettings>())
                // Database sql server                
                .AddDbContext(Configuration)
                // Servizi di infrastruttura
                .AddInfrastructureServices(Configuration)
                // Servizi applicativi
                //.AddApplicationServices()
                // Servizi di identity
                .AddIdentity(Configuration)
                .AddElsaWorkflowServices(Configuration);

            // Configurazione cors per dashboard elsa
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string[]>())
                .WithExposedHeaders("Content-Disposition"))
            );
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHttpActivities();
            var supportedCultures = new[]
            {
                new CultureInfo("it-IT"),

            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("it-IT"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            });
        }
    }
}
