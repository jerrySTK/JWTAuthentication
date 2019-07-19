using System;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NG_Core_Auth.Helpers;
using NG_Core_Auth.JWT;
using NG_Core_Auth.Models.DAL;
using NG_Core_Auth.Models.Entities;
using NG_Core_Auth.ViewModels;
using NG_Core_Auth.ViewModels.Validations;

namespace NG_Core_Auth {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2).AddFluentValidation();

            services.AddDbContext<SecurityDbContext> (options => {
                options.UseSqlServer (Configuration.GetConnectionString ("DefaultConnection"), b => b.MigrationsAssembly ("NG_Core_Auth"));
            });

            services.AddIdentity<AppUser, AppRole> ().AddEntityFrameworkStores<SecurityDbContext> ().AddDefaultTokenProviders ();

            services.AddScoped<IJwtFactory,JwtFactory>();
            services.AddTransient<IValidator<CredentialsViewModel>,CredentialsViewModelValidator>();

            services.Configure<IdentityOptions> (options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddAutoMapper();

            configureJWT(services);
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

            app.UseAuthentication ();

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

        private void configureJWT (IServiceCollection services) {
            var jwtAppSettings = Configuration.GetSection (nameof (JwtIssuerOptions));
            var _signingkey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (Configuration.GetSection ("JwtSecurity") ["SecretKey"]));

            services.Configure<JwtIssuerOptions> (options => {
                options.Issuer = jwtAppSettings[nameof (JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettings[nameof (JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials (_signingkey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettings[nameof (JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettings[nameof (JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingkey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication (options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer (jwtBearerOptions => {
                jwtBearerOptions.ClaimsIssuer = jwtAppSettings[nameof (JwtIssuerOptions.Issuer)];
                jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
                jwtBearerOptions.SaveToken = true;
            });

            services.AddAuthorization (options => {
                options.AddPolicy ("ApiUser", policy => policy.RequireClaim (ClaimTypes.Role, Constants.Strings.JwtClaims.ApiAccess));
            });
        }
    }
}