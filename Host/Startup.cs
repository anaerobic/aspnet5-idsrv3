using Autofac.Builder;
using Host.Configuration;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Owin.Security.DataProtection;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Logging.LogProviders;
using Thinktecture.IdentityServer.Core.Services;

namespace Host
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInstance(typeof(IDataProtectionProvider),
                new MonoDataProtectionProvider("idsrv3")); //services.Properties["host.AppName"] as string
        }

        //This method is invoked when ASPNET_ENV is 'Development' or is not defined
        //The allowed values are Development,Staging and Production
        public void ConfigureDevelopment(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            //Display custom error page in production when error occurs
            //During development use the ErrorPage middleware to display error information in the browser
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            // Add the runtime information page that can be used by developers
            // to see what packages are used by the application
            // default path is: /runtimeinfo
            app.UseRuntimeInfoPage();

            Configure(app);
        }

        //This method is invoked when ASPNET_ENV is 'Staging'
        //The allowed values are Development,Staging and Production
        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            //app.UseErrorHandler("/Home/Error");

            app.UseErrorPage(ErrorPageOptions.ShowAll);

            Configure(app);
        }

        //This method is invoked when ASPNET_ENV is 'Production'
        //The allowed values are Development,Staging and Production
        public void ConfigureProduction(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            //app.UseErrorHandler("/Home/Error");

            app.UseErrorPage(ErrorPageOptions.ShowAll);

            Configure(app);
        }

        public void Configure(IApplicationBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new ColouredConsoleLogProvider());
            //LogProvider.SetCurrentLogProvider(new TraceSourceLogProvider());
            
            app.UseStaticFiles();

            app.Map("/core", core =>
            {
                var factory = InMemoryFactory.Create(
                                users: Users.Get(),
                                clients: Clients.Get(),
                                scopes: Scopes.Get()
                                );

                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://idsrv3.com",
                    SiteName = "Thinktecture IdentityServer v3",
                    Factory = factory,
                    RequireSsl = false,
                    SigningCertificate = Certificate.Get(),
                    CorsPolicy = CorsPolicy.AllowAll
                };

                factory.TokenSigningService = new Registration<ITokenSigningService>(r => new FooTokenSigningService(idsrvOptions));


                core.UseIdentityServer(idsrvOptions);
            });
        }
    }
}
