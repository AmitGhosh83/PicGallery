using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KudVenvat1.Models;
using EmployeeManagement.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using PicGallery.DataAccess.Models;
using System.Runtime;
using System.Security.Claims;
using PicGallery.Security;
using Microsoft.CodeAnalysis.Options;

namespace KudVenvat1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Add Policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                                 policy => policy.RequireClaim("Delete Role"));

                options.AddPolicy("AdminRolePolicy",
                                policy => policy.RequireRole("Admin"));
                options.AddPolicy("EditRolePolicy",
                                policy => policy.RequireClaim("Edit Role"));

                //options.AddPolicy("EditRolePolicy",
                //                policy => policy.RequireAssertion(context =>
                //                     context.User.IsInRole("TestRole") && // User has Test Role + Edit Role Claim
                //                     context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Type == "true") ||
                //                     context.User.IsInRole("Admin")));// Either user has Admin role

                options.AddPolicy("EditRolePolicy",
                            policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

            });

            //Ensure everyone has to login to access 
            services.AddControllersWithViews( options=> {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy)); 
            });

            //Add Google Authetication
            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = Configuration["Google:ClientId"];
                        options.ClientSecret = Configuration["Google:ClientSecret"];
                    })
                    .AddFacebook(options =>
                    {
                        options.AppId = Configuration["Facebook:AppId"];
                        options.AppSecret = Configuration["Facebook:AppSecret"]; ;
                    });

            //Change Access Denied path
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Administration/AccessDenied");
            });


            services.AddDbContextPool<AppDbContext>(
                options=>
                options.UseSqlServer(Configuration.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();


            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
            //services.AddSingleton<IEmpRepository, InMemoryEmployeeRepository>();
            services.AddScoped<IEmpRepository, SQLEmployeeRepository>();

            //To bring in SecurityPolicy that 1 admin cant edit his own roles and claims
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesandClaimHandler>();
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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
