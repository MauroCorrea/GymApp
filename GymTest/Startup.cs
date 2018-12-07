using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GymTest.Data;
using Microsoft.EntityFrameworkCore;
using GymTest.Services;
using GymTest.Models;
using System.Globalization;

namespace GymTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //*******Services Injection*******
            services.AddScoped<IAssistanceLogic, AssistanceLogicImpl>();

            //*******Services Injection*******
            services.AddScoped<ISendEmail, SendEmailImpl>();

            //*******Database context implementation*******
            // Windows
            //services.AddDbContext<GymTestContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("GymTestContext")));
            //MAC
            services.AddDbContext<GymTestContext>(options =>
                    options.UseSqlite("Data Source=GymApp.db"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var cultureInfo = new CultureInfo("es-UY");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
