using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ejercicio2
{
    class encriptar
    {
        public static byte[] GenerateSalt()
        {
            const int saltLenght = 32;
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltLenght];
                randomNumberGenerator.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            //se crea un arreglo con la longitud de ambas cadenas a combinar
            var ret = new byte[first.Length + second.Length];

            //que voy a copiar, digito verificador, en donde, donde inicio, donde termino
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            //retornar el arreglo generado
            return ret;

        }

        public static byte[] HashPasswordWithSalth(byte[] toBeHashed, byte[] salt)
        {
            //se usa sha256 para encriptar
            using (var sha256 = SHA256.Create())
            {
                //se combinan ambos arreglos
                var combinedHash = Combine(toBeHashed, salt);
                //se encripta la combinacion
                return sha256.ComputeHash(combinedHash);
            }
        }

    }
}
