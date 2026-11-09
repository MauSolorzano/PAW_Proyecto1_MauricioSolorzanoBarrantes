using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Data
{
    public class ComentarioData
    {
        // Consultar los comentarios de un ticket
        public List<Comentario> ObtenerPorTicket(int idTicket)
        {
            // Crear una lista para almacenar los comentarios obtenidos
            List<Comentario> lista = new List<Comentario>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener los comentarios de un ticket específico
                string consulta = @"
                    SELECT C.Id, C.IdTicket, C.IdUsuario,
                           C.Texto, C.Fecha, U.Nombre
                    FROM Comentarios C
                    INNER JOIN Usuarios U ON C.IdUsuario = U.Id
                    WHERE C.IdTicket = @IdTicket
                    ORDER BY C.Fecha ASC";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);
                // Agregar el parámetro necesario para la consulta SQL
                comando.Parameters.AddWithValue("@IdTicket", idTicket);

                conexion.Open();
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader lector = comando.ExecuteReader();
                // Recorrer los resultados y mapearlos a objetos Comentario
                while (lector.Read())
                {
                    // Crear un nuevo objeto Comentario y agregarlo a la lista
                    lista.Add(new Comentario
                    {
                        Id = Convert.ToInt32(lector["Id"]),
                        IdTicket = Convert.ToInt32(lector["IdTicket"]),
                        IdUsuario = Convert.ToInt32(lector["IdUsuario"]),
                        Texto = lector["Texto"].ToString(),
                        Fecha = Convert.ToDateTime(lector["Fecha"]),
                        NombreUsuario = lector["Nombre"].ToString()
                    });
                }
            }
            // Devolver la lista de comentarios obtenidos
            return lista;
        }

        // Registrar un comentario de un ticket
        public void Crear(Comentario comentario)
        {
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para insertar un nuevo comentario en la tabla Comentarios
                string consulta = @"
                    INSERT INTO Comentarios
                        (IdTicket, IdUsuario, Texto)
                    VALUES
                        (@IdTicket, @IdUsuario, @Texto)";

                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);
                // Agregar los parámetros necesarios para la consulta SQL
                comando.Parameters.AddWithValue("@IdTicket", comentario.IdTicket);
                comando.Parameters.AddWithValue("@IdUsuario", comentario.IdUsuario);
                comando.Parameters.AddWithValue("@Texto", comentario.Texto);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}