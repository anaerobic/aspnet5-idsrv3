using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Thinktecture.IdentityServer.Core.Configuration;
using AspNet5Host.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Reflection;

namespace AspNet5Host
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            var idsrv3test = "idsrv3test.pfx";
            var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, idsrv3test);
            if (!File.Exists(certFile))
            {
                var localFile = Path.Combine(Environment.CurrentDirectory, idsrv3test);
                if (!File.Exists(localFile))
                {
                    localFile = Path.Combine(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location), idsrv3test);
                }

                File.Copy(localFile, certFile);
            }

            app.Map("/core", core =>
        {
            var factory = InMemoryFactory.Create(
                            users: Users.Get(),
                            clients: Clients.Get(),
                            scopes: Scopes.Get());

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = factory,
                RequireSsl = false,
                SigningCertificate = new X509Certificate2(certFile, "idsrv3test")
            };

            core.UseIdentityServer(idsrvOptions);
        });
        }
    }
}
