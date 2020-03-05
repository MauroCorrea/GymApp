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
using System;

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

            var appsettings = _configuration.GetSection("AppSettings");
            var settings = appsettings.Get<AppSettings>();

            services.Configure<AppSettings>(appsettings);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //*******Services Injection*******
            services.AddScoped<IAssistanceLogic, AssistanceLogicImpl>();

            //*******Services Injection*******
            services.AddScoped<ISendEmail, SendEmailImpl>();

            //*******Services Injection*******
            services.AddScoped<IPaymentLogic, PaymentLogicImpl>();

            //*******Services Injection*******
            services.AddScoped<IPaymentNotificationLogic, PaymentNotificationLogicImpl>();

            //*******Services Injection*******
            services.AddScoped<ITimezoneLogic, TimeZoneLogicImpl>();

            //*******Database context implementation*******
            services.AddDbContext<GymTestContext>(options =>
                options.UseMySql(settings.MySQLConfiguration.ConnectionString,
                        mysqlOptions =>
                        {
                            mysqlOptions.ServerVersion(settings.MySQLConfiguration.Version);
                        }
                ));
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
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
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
