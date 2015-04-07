using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Host.Configuration
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            const string idsrv3Test = "idsrv3test.pfx";
            
            var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, idsrv3Test);
            Console.WriteLine("current domain: " + certFile);
            if (!File.Exists(certFile))
            {
                var localFile = Path.Combine(Environment.CurrentDirectory, idsrv3Test);
                Console.WriteLine("current environment: " + localFile);
                if (!File.Exists(localFile))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    localFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), idsrv3Test);
                    Console.WriteLine("assembly location: " + localFile);
                }

                File.Copy(localFile, certFile);
            }

            return new X509Certificate2(certFile, Path.GetFileNameWithoutExtension(idsrv3Test),
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
        }
    }
}