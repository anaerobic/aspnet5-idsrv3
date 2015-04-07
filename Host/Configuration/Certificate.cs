using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Thinktecture.IdentityServer.Core.Logging;

namespace Host.Configuration
{
    static class Certificate
    {
        private const string PfxPwd = "idsrv3test";
        private const string PfxName = "device.pfx";

        public static X509Certificate2 Get()
        {
            var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PfxName);
            LogProvider.GetCurrentClassLogger().Info("current domain: " + certFile);

            if (!File.Exists(certFile))
            {
                var localFile = Path.Combine(Environment.CurrentDirectory, PfxName);
                LogProvider.GetCurrentClassLogger().Info("current environment: " + localFile);

                if (!File.Exists(localFile))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    localFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PfxName);
                    LogProvider.GetCurrentClassLogger().Info("assembly location: " + localFile);
                }

                File.Copy(localFile, certFile);
            }

            var cert = new X509Certificate2(certFile, PfxPwd,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            LogProvider.GetCurrentClassLogger().Info("HasPrivateKey: " + cert.HasPrivateKey);
            if (cert.HasPrivateKey)
                LogProvider.GetCurrentClassLogger().Info("PrivateKey.SignatureAlgorithm: " + cert.PrivateKey.SignatureAlgorithm);


            return cert;
        }
    }
}