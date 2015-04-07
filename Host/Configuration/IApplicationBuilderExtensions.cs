﻿using System;
using Thinktecture.IdentityServer.Core.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Owin;
using Microsoft.Framework.DependencyInjection;


// ReSharper disable once CheckNamespace
namespace Microsoft.AspNet.Builder
{

    using DataProtectionProviderDelegate = Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
    using DataProtectionTuple = Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>;

    // ReSharper disable once InconsistentNaming
    public static class IApplicationBuilderExtensions
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IdentityServerOptions options)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var builder = new Microsoft.Owin.Builder.AppBuilder();

                   // builder.SetDataProtectionProvider(new MonoDataProtectionProvider(app.Properties["host.AppName"] as string));

                    var provider = app.ApplicationServices.GetService<Microsoft.Owin.Security.DataProtection.IDataProtectionProvider>();

                    builder.Properties["security.DataProtectionProvider"] = new DataProtectionProviderDelegate(purposes =>
                    {
                        var dataProtection = provider.Create(String.Join(",", purposes));
                        return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                    });

                    builder.UseIdentityServer(options);

                    var appFunc = builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as Func<IDictionary<string, object>, Task>;
                    return appFunc;
                });
            });
        }
    }
}