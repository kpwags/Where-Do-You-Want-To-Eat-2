using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WhereDoYouWantToEat2.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WhereDoYouWantToEat2.Models;
using WhereDoYouWantToEat2.Classes;
using WhereDoYouWantToEat2.Services;
using WhereDoYouWantToEat2.Services.Interfaces;
using WhereDoYouWantToEat2.Respositories;
using WhereDoYouWantToEat2.Respositories.Interfaces;

namespace WhereDoYouWantToEat2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AppSettings config = new AppSettings();
            configuration.GetSection("Settings").Bind(config);
            Utilities.AppSettings = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.GetConnectionString("WhereDoYouWantToEat")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/login";
                options.LogoutPath = $"/logout";
            });

            services.AddTransient<IRepository<Restaurant>, Repository<Restaurant>>();
            services.AddTransient<IRepository<RestaurantTag>, Repository<RestaurantTag>>();
            services.AddTransient<IRepository<Tag>, Repository<Tag>>();
            services.AddTransient<IHomeService, HomeService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserProvider, UserProvider>();
            services.AddControllersWithViews();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseRewriter(new RewriteOptions()
                    // redirect non www to www.
                    .AddRedirectToWwwPermanent()

                    // While we are at it, let's also redirect http to https.
                    .AddRedirectToHttpsPermanent()
                );
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            loggerFactory.AddFile("Logs/where_do_you_want_to_eat-{Date}.log");
        }
    }
}
