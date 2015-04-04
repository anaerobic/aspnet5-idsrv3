using AspNet5Host;
using AspNet5Host.Configuration;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;

namespace AspNet5Host
{
    public class Startup_Mono
    {
        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            //LogProvider.SetCurrentLogProvider(new TraceSourceLogProvider());

            app.SetDataProtectionProvider(new MonoDataProtectionProvider(app.Properties["host.AppName"] as string));

            var factory = InMemoryFactory.Create(
                users: Users.Get(),
                clients: Clients.Get(),
                scopes: Scopes.Get());

            var options = new IdentityServerOptions
            {
                IssuerUri = "https://idsrv3.com",
                SiteName = "Thinktecture IdentityServer v3",
                SigningCertificate = Certificate.Get(),
                Factory = factory,
                CorsPolicy = CorsPolicy.AllowAll,
                RequireSsl = false,
            };

            app.UseIdentityServer(options);
        }
    }
}