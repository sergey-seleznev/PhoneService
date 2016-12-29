using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Proekspert.PhoneServiceTask.Host
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        protected virtual PhoneService CreatePhoneServiceInstance(IServiceProvider sp)
        {
            // get configuration option providers
            var legacyDataSourceOptions = sp.GetService<IOptions<LegacyDataSourceOptions>>()?.Value;
            var languages = sp.GetService<IOptions<LanguageOptions>>()?.Value?.Languages;
            if (legacyDataSourceOptions == null || languages == null) return null;

            // create LegacyDataProvider instance
            ILegacyDataProvider legacyDataProvider = new WebLegacyDataProvider(
                legacyDataSourceOptions.FileUriFormat,
                legacyDataSourceOptions.FirstIndex,
                legacyDataSourceOptions.LastIndex);

            // create PhoneService instance
            PhoneService phoneService = new PhoneService(
                legacyDataProvider, languages,
                legacyDataSourceOptions.UpdateIntervalMSec);

            // get SignalR service
            var signalRConnectionManager = sp.GetService<IConnectionManager>();

            // add PhoneService data updated handler
            phoneService.DataUpdated += async (sender, args) =>
                await signalRConnectionManager
                    .GetHubContext<PhoneServiceHub>()
                    .Clients.All.updateData();

            return phoneService;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // add framework services
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    // omit JSON null entries
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; 
                });
            
            // add SignalR
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            // add option providers
            services.Configure<LanguageOptions>(Configuration.GetSection("LanguageOptions"));
            services.Configure<LegacyDataSourceOptions>(Configuration.GetSection("LegacyDataSourceOptions"));

            // add PhoneService
            services.AddSingleton<IPhoneService, PhoneService>(CreatePhoneServiceInstance);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // use features
            app.UseStaticFiles();   // for JS libraries
            app.UseWebSockets();    // for effective SignalR
            app.UseSignalR();       // for data update notification
            
            // add default route
            app.UseMvc(routes =>
            {
                // >>> /
                routes.MapRoute(
                    name: "default",
                    template: "{controller=PhoneService}/{action=Index}");

                // >>> /data/
                routes.MapRoute(
                    name: "phoneServiceData",
                    template: "data/",
                    defaults: new { controller = "PhoneService", action = "Data" });

            });
        }
    }
}
