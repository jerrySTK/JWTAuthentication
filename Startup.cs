using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NG_Core_Auth.Models.DAL;
using NG_Core_Auth.Models.Entities;

namespace NG_Core_Auth {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

            services.AddDbContext<SecurityDbContext> (options => {
                options.UseSqlServer (Configuration.GetConnectionString ("DefaultConnection"), b => b.MigrationsAssembly ("NG_Core_Auth"));
            });

            services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<SecurityDbContext>().AddDefaultTokenProviders();
            
            services.Configure<IdentityOptions>(options=>{
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddAutoMapper();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles (configuration => {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseStaticFiles ();
            app.UseSpaStaticFiles ();

            app.UseAuthentication();

            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            
            app.UseSpa (spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment ()) {
                    spa.UseAngularCliServer (npmScript: "start");
                }
            });
        }
    }
}