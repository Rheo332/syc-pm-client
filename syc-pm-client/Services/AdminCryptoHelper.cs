using System;
using System.Security.Cryptography;
using System.Text;

namespace syc_pm_client.Services
{
    public static class AdminCryptoHelper
    {
        public static string EncryptPassword(string plainText, string adminPublicKeyBase64)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            if (string.IsNullOrEmpty(adminPublicKeyBase64)) return plainText; // or throw?

            byte[] publicKeyBytes;
            try
            {
                publicKeyBytes = Convert.FromBase64String(adminPublicKeyBase64);
            }
            catch
            {
                return string.Empty;
            }

            using var rsa = RSA.Create();
            try
            {
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            }
            catch
            {
                try
                {
                    rsa.ImportRSAPublicKey(publicKeyBytes, out _);
                }
                catch
                {
                    return string.Empty;
                }
            }

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}