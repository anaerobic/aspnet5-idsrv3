using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.Core.Logging;

namespace Host.Configuration
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            const string idsrv3Test = "idsrv3test.pfx";

            var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, idsrv3Test);
            LogProvider.GetCurrentClassLogger().Info("current domain: " + certFile);

            if (!File.Exists(certFile))
            {
                var localFile = Path.Combine(Environment.CurrentDirectory, idsrv3Test);
                LogProvider.GetCurrentClassLogger().Info("current environment: " + localFile);

                if (!File.Exists(localFile))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    localFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), idsrv3Test);
                    LogProvider.GetCurrentClassLogger().Info("assembly location: " + localFile);
                }

                File.Copy(localFile, certFile);
            }

            var cert = new X509Certificate2(certFile, Path.GetFileNameWithoutExtension(idsrv3Test),
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            LogProvider.GetCurrentClassLogger().Info("HasPrivateKey: " + cert.HasPrivateKey);
            if (cert.HasPrivateKey)
                LogProvider.GetCurrentClassLogger().Info("PrivateKey.SignatureAlgorithm: " + cert.PrivateKey.SignatureAlgorithm);


            return cert;
        }
    }
}