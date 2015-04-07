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
        private const string CertPwd = "idsrv3test";
        private const string KeyName = "device.key";
        private const string PemName = "device.pem";
        private const string PfxName = "device.pfx";

        public static X509Certificate2 Get()
        {
            var certFile = GetAccessibleFilePath(PfxName);

            var cert = new X509Certificate2(certFile, CertPwd,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            LogProvider.GetCurrentClassLogger().Info("HasPrivateKey: " + cert.HasPrivateKey);
            if (cert.HasPrivateKey)
                LogProvider.GetCurrentClassLogger().Info("PrivateKey.SignatureAlgorithm: " + cert.PrivateKey.SignatureAlgorithm);
            
            return cert;
        }

        public static X509Certificate2 GetCertificateFromPEMstring()
        {
            var cert = File.ReadAllText(GetAccessibleFilePath(PemName));
            var certBuffer = Helpers.GetBytesFromPEM(cert, PemStringType.Certificate);

            var key = File.ReadAllText(GetAccessibleFilePath(KeyName));
            var keyBuffer = Helpers.GetBytesFromPEM(key, PemStringType.RsaPrivateKey);

            var certificate = new X509Certificate2(certBuffer, CertPwd);
            
            var prov = Crypto.DecodeRsaPrivateKey(keyBuffer);
            certificate.PrivateKey = prov;

            LogProvider.GetCurrentClassLogger().Info("HasPrivateKey: " + certificate.HasPrivateKey);
            if (certificate.HasPrivateKey)
                LogProvider.GetCurrentClassLogger().Info("PrivateKey.SignatureAlgorithm: " + certificate.PrivateKey.SignatureAlgorithm);

            return certificate;
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