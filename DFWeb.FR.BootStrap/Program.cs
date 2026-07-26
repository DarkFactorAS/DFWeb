using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DFCommonLib.Logger;
using DFCommonLib.Utils;
using DFCommonLib.Config;
using DFCommonLib.DataAccess;

using DFWeb.BE.Repository;
using DFWeb.BE.ConfigModel;
using DFWeb.BE.Provider;

using AccountClientModule.Client;

using DFWeb.BE;

namespace DarkFactorCoreNet
{
    public class Program
    {
        public static readonly string AppName = "DarkFactor Web";
        public static readonly string Version = "1.0.0";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                DFWebBELibrary.ConfigureServices(hostContext, services);

                //services.Configure<DatabaseConfig>(Configuration.GetSection("DatabaseConfigModel"));
                //services.Configure<EmailConfiguration>(Configuration.GetSection("EmailConfiguration"));
                services.AddTransient<IConfigurationHelper, ConfigurationHelper<WebConfig> >();


                services.AddScoped(typeof(IMenuProvider), typeof(MenuProvider));
                services.AddScoped(typeof(IPageProvider), typeof(PageProvider));
                services.AddScoped(typeof(IEditPageProvider), typeof(EditPageProvider));
                services.AddScoped(typeof(ILoginRepository), typeof(LoginRepository));

                services.AddScoped(typeof(ILoginProvider), typeof(LoginProvider));
                services.AddScoped(typeof(IUserSessionProvider), typeof(UserSessionProvider));
                services.AddScoped(typeof(IImageProvider), typeof(ImageProvider));

                services.AddScoped(typeof(IMenuRepository), typeof(MenuRepository));
                services.AddScoped(typeof(IPageRepository), typeof(PageRepository));
                services.AddScoped(typeof(IEditPageRepository), typeof(EditPageRepository));
                services.AddScoped(typeof(IImageRepository), typeof(ImageRepository));

                services.AddScoped(typeof(ICookieProvider), typeof(CookieProvider));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
