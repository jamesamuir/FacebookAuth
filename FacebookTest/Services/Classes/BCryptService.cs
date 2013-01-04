using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookTest.Services.Classes
{
    internal class BCryptService
    {
        public string GenerateToken()
        {
            return Guid.NewGuid().ToString()
                             .ToLowerInvariant()
                             .Replace("-", "");
        }

        /// <summary>
        /// Hashes the identifier with the provided salt.
        /// </summary>
        /// <param name="identifier">The identifier to hash.</param>
        /// <returns>The hashed identifier.</returns>
        public string Hash(string identifier)
        {
            const string pepper = "BCryptSecurityEncoder";
            string salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor: 10);

            return BCrypt.Net.BCrypt.HashPassword(identifier, salt + pepper);
        }

        /// <summary>
        /// Verifies if the identifier matches the hash.
        /// </summary>
        /// <param name="identifier">The identifier to check.</param>
        /// <param name="hash">The hash to check against.</param>
        /// <returns>true if the identifiers match, false otherwise.</returns>
        public bool Verify(string identifier, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(identifier, hash);
        }

    }
}