using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tyche.Scanner.Models;
using Tyche.Scanner.Workers;

namespace Tyche.Scanner
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var settingsProvider = new SettingsProvider(Path.Combine(AppContext.BaseDirectory, "settings.json"));
            services.AddScoped(options => settingsProvider);
            services.AddSingleton(options => new TaskWorker());
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Scan folder service. Use API calls to interact with it."));
            });
        }
    }
}
