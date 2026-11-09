using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Data
{
    public class Seguridad
    {
        //Metodo para crear un hash de la contraseña utilizando PBKDF2 con una sal aleatoria.
        public static string CrearHash(string contrasena)
        {
            byte[] salt = new byte[16];

            using (var generador = RandomNumberGenerator.Create())
            {
                generador.GetBytes(salt);
            }

            using (var algoritmo = new Rfc2898DeriveBytes(
                contrasena, salt, 100000))
            {
                byte[] hash = algoritmo.GetBytes(32);

                return Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }
        }
        //Metodo para verificar si la contraseña ingresada coincide con el hash almacenado.
        public static bool VerificarContrasena(
                string contrasena, string hashGuardado)
        {
            string[] partes = hashGuardado.Split(':');

            if (partes.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(partes[0]);
            byte[] hashOriginal = Convert.FromBase64String(partes[1]);

            using (var algoritmo = new Rfc2898DeriveBytes(
                contrasena, salt, 100000))
            {
                byte[] hashIngresado = algoritmo.GetBytes(32);

                return CompararBytes(hashOriginal, hashIngresado);
            }
        }
        //Metodo privado para comparar dos arreglos de bytes de manera segura.
        private static bool CompararBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int diferencia = 0;

            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }

            return diferencia == 0;
        }
    }
}