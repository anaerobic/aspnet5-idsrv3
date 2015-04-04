using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace AspNet5Host.Configuration
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            const string idsrv3Test = "idsrv3test.pfx";

            var assembly = typeof(Certificate).Assembly;
            using (var stream = assembly.GetManifestResourceStream("SelfHost.Config." + idsrv3Test))
            {
                if (stream != null && stream.Length > 0)
                {
                    return new X509Certificate2(ReadStream(stream), Path.GetFileNameWithoutExtension(idsrv3Test));
                }

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

                return new X509Certificate2(certFile, Path.GetFileNameWithoutExtension(idsrv3Test));
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}