using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiAuth.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiAuth.Models;

namespace WebApiEFCore {
    public class Startup {
        public Startup (IHostingEnvironment env) {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : false, reloadOnChange : true)
                .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional : true)
                .AddEnvironmentVariables ();
            Configuration = builder.Build ();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            // Add framework services.
            var connection = @"Server=(localdb)\mssqllocaldb;Database=DBEF;Trusted_Connection=True;";

            services.AddDbContext<DBContext> (options => options.UseSqlServer (connection,
                optionsBuilder => optionsBuilder.MigrationsAssembly ("WebApiEFCore")));

            services.AddIdentity<Usuario, IdentityRole> ()
                .AddEntityFrameworkStores<DBContext> ()
                .AddDefaultTokenProviders ();

            services.Configure<IdentityOptions> (o => {
                o.SignIn.RequireConfirmedEmail = true;
                o.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours (2);
            });
            services.AddTransient<IMessageService, FileMessageService> ();

            services.AddTransient<IRolesService, RolesService> ();

            services.AddCors ();

            services.AddMvc ().AddJsonOptions (
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole (Configuration.GetSection ("Logging"));
            loggerFactory.AddDebug ();
            app.UseCors (b => b.WithOrigins ("http://localhost.com:4000")
                .AllowAnyOrigin ()
                .AllowCredentials ()
                .AllowAnyMethod ()
                .AllowAnyHeader ());
            app.UseIdentity ();
            app.UseMvc ();
        }
    }
}