using ElasticSearchExample.Data.DAL;
using ElasticSearchExample.Data.DAL.Repository;
using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Web.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<PersonContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PersonConnection"),sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<PersonContext, PersonContext>();
            //services.AddScoped<Microsoft.Extensions.Hosting.IHostedService, ElasticSearchCreateIndexService>();
            services.AddHostedService<ElasticSearchCreateIndexService>();
            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,PersonContext context)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
