using System.Security.Cryptography;
using ApiGatewayProcesarSms.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApiGatewayProcesarSms.Services
{
    internal class HashService
    {
        public HashResult Hash(string input)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Hash(input, salt);
        }

        public HashResult Hash(string input, byte[] salt)
        {
            string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: input,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));
            return new HashResult()
            {
                Hash = hashed,
                salt = salt
            };
        }        
    }
}
