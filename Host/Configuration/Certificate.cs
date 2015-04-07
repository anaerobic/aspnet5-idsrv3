using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.Core.Logging;

namespace Host.Configuration
{
    static class Certificate
    {
        private const string CertPwd = "idsrv3test";
        private const string KeyName = "device.key";
        private const string PemName = "device.pem";
        private const string PfxName = "device.pfx";

        public static X509Certificate2 GetX509()
        {
            var x509 = GetX509FromPfx();

            LogProvider.GetCurrentClassLogger().Info("HasPrivateKey: " + x509.HasPrivateKey);

            if (x509.HasPrivateKey)
                LogProvider.GetCurrentClassLogger().Info("PrivateKey.SignatureAlgorithm: " + x509.PrivateKey.SignatureAlgorithm);

            return x509;
        }

        private static X509Certificate2 GetX509FromPfx()
        {
            var certFile = GetAccessibleFilePath(PfxName);

            var cert = new X509Certificate2(certFile, CertPwd,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            return cert;
        }

        private static X509Certificate2 GetX509FromPemAndKey()
        {
            var cert = File.ReadAllText(GetAccessibleFilePath(PemName));
            var certBuffer = Helpers.GetBytesFromPEM(cert, PemStringType.Certificate);

            var key = File.ReadAllText(GetAccessibleFilePath(KeyName));
            var keyBuffer = Helpers.GetBytesFromPEM(key, PemStringType.RsaPrivateKey);

            var newCert = new X509Certificate2(certBuffer, CertPwd);

            var prov = Crypto.DecodeRsaPrivateKey(keyBuffer);
            newCert.PrivateKey = prov;

            return newCert;
        }

        public static string GetAccessibleFilePath(string fileName)
        {
            var needFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            LogProvider.GetCurrentClassLogger().Info("current domain: " + needFile);

            if (!File.Exists(needFile))
            {
                var haveFile = Path.Combine(Environment.CurrentDirectory, fileName);
                LogProvider.GetCurrentClassLogger().Info("current environment: " + haveFile);

                if (!File.Exists(haveFile))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    haveFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
                    LogProvider.GetCurrentClassLogger().Info("assembly location: " + haveFile);
                }

                File.Copy(haveFile, needFile);
            }
            return needFile;
        }
    }
}