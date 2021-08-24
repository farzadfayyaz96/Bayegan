using System;
using System.Text;
using System.Security.Cryptography;
using Bayegan.Services.Abstractions;

namespace Bayegan.Services.Cryptography.Aes
{
    public class AesCryptoService : ICryptoService
    {
        private readonly AesCryptoServiceProvider _aesCryptoServiceProvider;

        public AesCryptoService(string key, string iv)
        {
            CheckKeyAndIvLength(key, iv);

            _aesCryptoServiceProvider = GenerateAes(key, iv);
        }

        private void CheckKeyAndIvLength(string key, string iv)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            
            if(string.IsNullOrEmpty(iv))
                throw new ArgumentNullException(nameof(iv));

            if(key.Length != 32)
                throw new ArgumentException("invalid 'key' length. 'key' length is 32 chars.");

            if(iv.Length != 16)
                throw new ArgumentException("invalid 'iv' length. 'iv' length is 16 chars.");
        }

        private AesCryptoServiceProvider GenerateAes(string key, string iv)
        {
            return new AesCryptoServiceProvider
            {
                BlockSize= 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
        }

        public string Descrypt(string textToDecrypt)
        {
            if(string.IsNullOrEmpty(textToDecrypt))
            {
                throw new ArgumentNullException(nameof(textToDecrypt));
            }

            var txtByteData = Convert.FromBase64String(textToDecrypt);
            var cryptoTransform = _aesCryptoServiceProvider.CreateDecryptor();

            var result = cryptoTransform.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return Encoding.ASCII.GetString(result);
        }

        public string Encrypt(string textToEncrypt)
        {
            if(string.IsNullOrEmpty(textToEncrypt))
            {
                throw new ArgumentNullException(nameof(textToEncrypt));
            }

            var txtByteData = Encoding.ASCII.GetBytes(textToEncrypt);
            var cryptoTransform = _aesCryptoServiceProvider.CreateEncryptor(_aesCryptoServiceProvider.Key, _aesCryptoServiceProvider.IV);

            var result = cryptoTransform.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return Convert.ToBase64String(result);
        }
    }

}

