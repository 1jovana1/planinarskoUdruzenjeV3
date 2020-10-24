using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using planinarskoUdruzenjeV3.Areas.Identity.Data;
using planinarskoUdruzenjeV3.Models;

[assembly: HostingStartup(typeof(planinarskoUdruzenjeV3.Areas.Identity.IdentityHostingStartup))]
namespace planinarskoUdruzenjeV3.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PlaninarskoUdruzenjeContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PlaninarskoUdruzenjeContext")));

                services.AddDefaultIdentity<User>(options => {
                    options.SignIn.RequireConfirmedAccount = false; //Kasnije true
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                }) 
                    .AddEntityFrameworkStores<PlaninarskoUdruzenjeContext>();
            });
        }
    }
}