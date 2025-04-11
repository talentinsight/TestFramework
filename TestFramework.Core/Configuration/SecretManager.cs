using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TestFramework.Core.Configuration
{
    /// <summary>
    /// Manages secrets for the test framework
    /// </summary>
    public class SecretManager
    {
        private readonly string _secretsPath;
        private readonly byte[] _encryptionKey;
        private Dictionary<string, string> _secrets;

        /// <summary>
        /// Initializes a new instance of the SecretManager class
        /// </summary>
        /// <param name="secretsPath">Path to the secrets file</param>
        /// <param name="encryptionKey">Key used for encryption (must be 32 bytes)</param>
        public SecretManager(string secretsPath, string encryptionKey)
        {
            _secretsPath = secretsPath;
            _encryptionKey = Convert.FromBase64String(encryptionKey);
            _secrets = new Dictionary<string, string>();
            LoadSecrets();
        }

        /// <summary>
        /// Gets a secret value by key
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <returns>Secret value if found, null otherwise</returns>
        public string? GetSecret(string key)
        {
            return _secrets.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Sets a secret value
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="value">Secret value</param>
        public void SetSecret(string key, string value)
        {
            _secrets[key] = value;
            SaveSecrets();
        }

        /// <summary>
        /// Removes a secret
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <returns>True if the secret was removed, false otherwise</returns>
        public bool RemoveSecret(string key)
        {
            var removed = _secrets.Remove(key);
            if (removed)
            {
                SaveSecrets();
            }
            return removed;
        }

        /// <summary>
        /// Gets all secret keys
        /// </summary>
        /// <returns>Collection of secret keys</returns>
        public IEnumerable<string> GetSecretKeys()
        {
            return _secrets.Keys;
        }

        private void LoadSecrets()
        {
            if (!File.Exists(_secretsPath))
            {
                _secrets = new Dictionary<string, string>();
                return;
            }

            var encryptedData = File.ReadAllBytes(_secretsPath);
            var json = Decrypt(encryptedData);
            _secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                      ?? new Dictionary<string, string>();
        }

        private void SaveSecrets()
        {
            var json = JsonSerializer.Serialize(_secrets);
            var encryptedData = Encrypt(json);
            File.WriteAllBytes(_secretsPath, encryptedData);
        }

        private byte[] Encrypt(string data)
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            
            var iv = aes.IV;
            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            
            // Write the IV first
            msEncrypt.Write(iv, 0, iv.Length);
            
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            return msEncrypt.ToArray();
        }

        private string Decrypt(byte[] encryptedData)
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            
            // Get the IV from the beginning of the data
            var iv = new byte[aes.IV.Length];
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;
            
            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }

        /// <summary>
        /// Generates a new encryption key
        /// </summary>
        /// <returns>Base64 encoded encryption key</returns>
        public static string GenerateEncryptionKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }
} 