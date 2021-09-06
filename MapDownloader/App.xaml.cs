using MapDownloader.Models;
using MapDownloader.Services.MapCache;
using MapDownloader.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MapDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;
        public static IServiceProvider ServiceProvider { get; private set; }

        private ITileDownload _mapDownload;

        public App()
        {
            host = Host.CreateDefaultBuilder()  // Use default settings
                                                //new HostBuilder()          // Initialize an empty HostBuilder
                    .ConfigureAppConfiguration((hostingContext, configuration) =>
                    {
                        // Add other configuration files...
                        configuration.Sources.Clear();

                        IHostEnvironment env = hostingContext.HostingEnvironment;

                        configuration
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    }).ConfigureServices((hostingContext, services) =>
                    {
                        ConfigureServices(hostingContext.Configuration, services);
                    })
                    .ConfigureLogging(logging =>
                    {
                        // Add other loggers...
                    })
                    .Build();

            ServiceProvider = host.Services;
        }
        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            _mapDownload = new HttpTileDownload();
            services.AddSingleton<ITileDownload>(_mapDownload);
            // Register all ViewModels.
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MapViewModel>();

            // Register all the Windows of the applications.
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync();

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }

    }
}
