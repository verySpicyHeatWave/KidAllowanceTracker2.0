using AllowanceApp.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace AllowanceApp.Avalonia.Utilities
{
    public static class CryptoUtils
    {
        private static readonly Lazy<string> PublicKeyPem = new(() =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "public.pem")));

        private static string _keyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AllowanceApp", "Keys");

        private static string GetFile(string extension)
        {
            if (!Directory.Exists(_keyDir))
                throw new InvalidOperationException($"Key directory does not exist: {_keyDir}");
            var files = Directory.EnumerateFiles(_keyDir, $"*{extension}", SearchOption.AllDirectories).ToList();
            if (files.Count != 1)
                throw new InvalidOperationException($"Only exactly one file of type {extension} should exist in the Key directory");

            return files.First();
        }

        public static string GetKeyFile() => GetFile(".pem");

        public static string GetLicenseFile() => GetFile(".lic");

        public static string GetSignatureFile() => GetFile(".sig");

        public static bool TryValidateSignature([NotNullWhen(true)]out ParentLicense? license)
        {
            bool resp = false;
            license = null;

            try
            {
                var key = GetKeyFile();
                var lic = GetLicenseFile();
                var sig = GetSignatureFile();

                using var rsa = RSA.Create();
                rsa.ImportFromPem(PublicKeyPem.Value);
                
                if (rsa.VerifyData(File.ReadAllBytes(lic), File.ReadAllBytes(sig), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {

                    var licText = File.ReadAllText(lic);
                    license = JsonSerializer.Deserialize<ParentLicense>(licText);

                    if (license is not null)
                    {
                        resp = true;
                    }
                }
            }
            catch
            {
                resp = false;
            }
            return resp;
        }
    }
}
