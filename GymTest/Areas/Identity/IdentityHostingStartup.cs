using System;
using GymTest.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(GymTest.Areas.Identity.IdentityHostingStartup))]
namespace GymTest.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "TyrUyIdentity";
                    options.Cookie.Expiration = TimeSpan.FromHours(context.Configuration.GetValue<Int16>("CookieHoursExpiration"));
                    options.ExpireTimeSpan = TimeSpan.FromHours(context.Configuration.GetValue<Int16>("CookieHoursExpiration"));
                    options.SlidingExpiration = true;
                });

                services.AddDbContext<GymTestIdentityDbContext>(options =>
                    options.UseMySql(
                        context.Configuration.GetConnectionString("GymTestIdentityDbContextConnection")));

                services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<GymTestIdentityDbContext>();
            });
        }
    }
}