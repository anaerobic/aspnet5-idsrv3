/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Host.Configuration
{
    /// <summary>
    /// Default token signing service
    /// </summary>
    public class FooTokenSigningService : ITokenSigningService
    {
        /// <summary>
        /// The identity server options
        /// </summary>
        protected readonly IdentityServerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FooTokenSigningService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public FooTokenSigningService(IdentityServerOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Signs the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// A protected and serialized security token
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Invalid token type</exception>
        public virtual Task<string> SignTokenAsync(Token token)
        {
            return Task.FromResult(CreateJsonWebToken(token, new X509SigningCredentials(_options.SigningCertificate)));
        }

        /// <summary>
        /// Creates the json web token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        protected virtual string CreateJsonWebToken(Token token, SigningCredentials credentials)
        {
            var jwt = new JwtSecurityToken(
                token.Issuer,
                token.Audience,
                token.Claims,
                DateTimeHelper.UtcNow,
                DateTimeHelper.UtcNow.AddSeconds(token.Lifetime),
                credentials);

            var x509credential = credentials as X509SigningCredentials;
            if (x509credential != null)
            {
                jwt.Header.Add("kid", Base64Url.Encode(x509credential.Certificate.GetCertHash()));
            }

            var x509 = credentials as X509SigningCredentials;

            var handler = new JwtSecurityTokenHandler
            {
                SignatureProviderFactory = new FooSignatureProviderFactory(x509.Certificate)
            };

            return handler.WriteToken(jwt);
        }

        internal static class DateTimeHelper
        {
            internal static DateTime UtcNow => DateTime.SpecifyKind(DateTimeOffsetHelper.UtcNow.DateTime, DateTimeKind.Utc);
        }
        internal static class DateTimeOffsetHelper
        {
            internal static Func<DateTimeOffset> UtcNowFunc = () => DateTimeOffset.UtcNow;

            internal static DateTimeOffset UtcNow => UtcNowFunc();
        }

        class FooSignatureProviderFactory : SignatureProviderFactory
        {
            private readonly X509Certificate2 _x509;

            public FooSignatureProviderFactory(X509Certificate2 x509)
            {
                _x509 = x509;
            }

            public override SignatureProvider CreateForSigning(SecurityKey key, string algorithm)
            {
                var signatureProvider = GetSymmetricSignatureProvider(algorithm);

                LogProvider.GetCurrentClassLogger().Info("current signing context: " + signatureProvider.Context);

                return signatureProvider;
            }

            public override SignatureProvider CreateForVerifying(SecurityKey key, string algorithm)
            {
                var signatureProvider = GetSymmetricSignatureProvider(algorithm);

                LogProvider.GetCurrentClassLogger().Info("current verifying context: " + signatureProvider.Context);

                return signatureProvider;
            }

            private static SymmetricSignatureProvider GetSymmetricSignatureProvider(string algorithm)
            {
                var encoding = new System.Text.ASCIIEncoding();
                var keyByte = encoding.GetBytes("idsrv3test");
                var myhmacsha1 = new HMACSHA1(keyByte);
                
                var signatureProvider = new SymmetricSignatureProvider(
                    new InMemorySymmetricSecurityKey(myhmacsha1.Key),
                    algorithm);
                return signatureProvider;
            }
        }
    }
}