using ElasticSearchExample.Data.DAL;
using ElasticSearchExample.Data.DAL.Repository;
using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Web.Helper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ElasticSearchExample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PersonContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PersonConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<PersonContext, PersonContext>();
            //ElasticSearch için yazýlan bir tane job
            services.AddHostedService<ElasticSearchCreateIndexService>();
            services.AddControllersWithViews();

            services.AddEntityFrameworkNpgsql().AddDbContext<PersonContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PersonConnection"));
            });

            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("PersonConnection")));
            services.AddHangfireServer();

            //        services.AddHangfire(x =>
            //x.UsePostgreSqlStorage(Configuration.GetConnectionString("PersonConnection")));
        }
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env, PersonContext context)
        {
            context.Database.EnsureCreated();
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

            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireDashboard("", new DashboardOptions
            {
                DashboardTitle = "Hangfire Testting",
                //Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
