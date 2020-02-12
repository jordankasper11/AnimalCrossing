using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimalCrossing.Web.Caching;
using AnimalCrossing.Web.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AnimalCrossing.Web
{
    public class Startup
    {
        private IWebHostEnvironment _env;

        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
                services.AddDistributedMemoryCache();
            else
            {
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = Environment.GetEnvironmentVariable("RedisUrl") ?? throw new ArgumentNullException("RedisUrl");
                    options.InstanceName = "AnimalCrossing|";
                });
            }

            services.AddTransient<CacheManager>();

            services.AddSingleton<VillagerRepository>(serviceProvider =>
            {
                return new VillagerRepository(Path.Combine(Directory.GetCurrentDirectory(), "data", "villagers.json"));
            });

            services.AddTransient<GameRepository>();
            services.AddControllers();
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "wwwroot");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/images"),
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "images"))
            });

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseSpaStaticFiles();

            app.UseSpa(builder =>
            {
                if (env.IsDevelopment())
                    builder.UseProxyToSpaDevelopmentServer($"http://localhost:4200/");
            });
        }
    }
}
