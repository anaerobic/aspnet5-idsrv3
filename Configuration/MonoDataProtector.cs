using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Owin.Security.DataProtection;

namespace AspNet5Host.Configuration
{public class MonoDataProtector : IDataProtector
    {
        private const string PRIMARY_PURPOSE = "Microsoft.Owin.Security.IDataProtector";

        private readonly string appName;
        private readonly DataProtectionScope dataProtectionScope;
        private readonly string[] purposes;

        public MonoDataProtector(string appName, string[] purposes)
        {
            if (appName == null) { throw new ArgumentNullException("appName"); }
            if (purposes == null) { throw new ArgumentNullException("purposes"); }

            this.appName = appName;
            this.purposes = purposes;
            this.dataProtectionScope = DataProtectionScope.CurrentUser;
        }

        public byte[] Protect(byte[] userData)
        {
            return ProtectedData.Protect(userData, this.GetEntropy(), dataProtectionScope);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return ProtectedData.Unprotect(protectedData, this.GetEntropy(), dataProtectionScope);
        }

        private byte[] GetEntropy()
        {
            using (var sha256 = SHA256.Create())
            {
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, sha256, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(this.appName);
                    writer.Write(PRIMARY_PURPOSE);

                    foreach (var purpose in this.purposes)
                    {
                        writer.Write(purpose);
                    }
                }

                return sha256.Hash;
            }
        }
    }
}