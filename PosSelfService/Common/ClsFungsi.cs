using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsFungsi
    {
        private string _EncPassPhrase; //untuk kepentingan enkripsi
        private string _EncSaltValue; // untuk kepentingan enkripsi

        public string Nb(object AnyValue)
        {
            return (AnyValue == null || AnyValue is DBNull) ? "" : AnyValue.ToString().Trim();
        }

        public string EncPassPhrase(DateTime TglInitial = default)
        {
            if (TglInitial == default)
            {
                TglInitial = DateTime.Now;
            }

            _EncPassPhrase = "MisterDonut " + TglInitial.ToString("yyyy-MM-dd");
            return _EncPassPhrase;
        }

        public string EncSaltValue
        {
            get
            {
                _EncSaltValue = "IDM";
                return _EncSaltValue;
            }
        }


        internal string Encrypt(string plainText, string passPhrase, string saltValue)
        {
            string hashAlgorithm = "SHA1";
            int passwordIterations = 2;
            string initVector = "@1B2c3D4e5F6g7H8"; // harus berukuran 16 byte
            int keySize = 256;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }

    }
}