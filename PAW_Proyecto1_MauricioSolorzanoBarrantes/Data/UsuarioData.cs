using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Data
{
    public class UsuarioData
    {
        //Este método busca un usuario por su email en la base de datos
        //y devuelve un objeto Usuario si se encuentra, o null si no se encuentra.
        public Usuario BuscarPorEmail(string email)
        {
            // Inicializamos la variable usuario como null. Si no se encuentra un usuario con el email proporcionado,
            Usuario usuario = null;
            // Se establece una conexión con la base de datos utilizando el método ObtenerConexion de la clase Conexion.
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Se define la consulta SQL para buscar un usuario por su email.
                string consulta = @"
                    SELECT Id, Nombre, Email, ContrasenaHash,
                           Rol, FechaRegistro, Estado
                    FROM Usuarios
                    WHERE Email = @Email";
                // Se crea un comando SQL con la consulta y la conexión.
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    // Se agrega el parámetro @Email al comando SQL para evitar inyecciones SQL.
                    comando.Parameters.AddWithValue("@Email", email);

                    conexion.Open();
                    // Se ejecuta el comando y se obtiene un lector de datos (SqlDataReader) para leer los resultados de la consulta.
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        // Si se encuentra un registro, se crea un objeto Usuario y se
                        // llenan sus propiedades con los datos obtenidos del lector.
                        if (lector.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = Convert.ToInt32(lector["Id"]),
                                Nombre = lector["Nombre"].ToString(),
                                Email = lector["Email"].ToString(),
                                ContrasenaHash = lector["ContrasenaHash"].ToString(),
                                Rol = lector["Rol"].ToString(),
                                FechaRegistro = Convert.ToDateTime(lector["FechaRegistro"]),
                                Estado = Convert.ToBoolean(lector["Estado"])
                            };
                        }
                    }
                }
            }
            // Se devuelve el objeto Usuario encontrado o null si no se encontró ningún usuario con el email proporcionado.
            return usuario;
        }
        //Obtener todos los usuarios con estado activo desde la base de datos.
        public List<Usuario> ObtenerUsuarios()
        {
            // Crear una lista para almacenar los usuarios obtenidos
            List<Usuario> lista = new List<Usuario>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener todos los usuarios con rol "Usuario" y estado activo
                string consulta = @"
                SELECT Id, Nombre, Email
                FROM Usuarios
              
                WHERE Estado = 1
                ORDER BY Nombre";
                // Crear un comando SQL para ejecutar la consulta

                //WHERE Rol = 'Usuario'

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    // Ejecutar la consulta y obtener un lector de datos
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        // Recorrer los resultados y mapearlos a objetos Usuario
                        while (lector.Read())
                        {
                            lista.Add(new Usuario
                            {
                                Id = Convert.ToInt32(lector["Id"]),
                                Nombre = lector["Nombre"].ToString(),
                                Email = lector["Email"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}