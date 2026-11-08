using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Data
{
    public class TicketData
    {
        // Obtener todos los tickets de la base de datos
        public List<Ticket> ObtenerTodos()
        {
            // Crear una lista para almacenar los tickets obtenidos
            List<Ticket> lista = new List<Ticket>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener todos los tickets de la tabla Tickets
                string consulta = @"
                    SELECT Id, Titulo, Descripcion,
                           IdUsuarioSolicitante, IdUsuarioAsignado,
                           IdEstado, FechaCreacion, FechaActualizacion,
                           Categoria
                    FROM Tickets
                    ORDER BY FechaCreacion DESC";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader lector = comando.ExecuteReader();
                // Recorrer los resultados y mapearlos a objetos Ticket
                while (lector.Read())
                {
                    lista.Add(MapearTicket(lector));
                }
            }
            //Se devuelve la lista 
            return lista;
        }

        //Registrar ticket
        public void Crear(Ticket ticket)
        {
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para insertar un nuevo ticket en la tabla Tickets
                string consulta = @"
                    INSERT INTO Tickets
                    (Titulo, Descripcion, IdUsuarioSolicitante,
                     IdEstado, Categoria)
                    VALUES
                    (@Titulo, @Descripcion, @IdUsuarioSolicitante,
                     @IdEstado, @Categoria)";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);
                // Agregar los parámetros necesarios para la consulta SQL
                comando.Parameters.AddWithValue("@Titulo", ticket.Titulo);
                comando.Parameters.AddWithValue("@Descripcion", ticket.Descripcion);
                comando.Parameters.AddWithValue(
                    "@IdUsuarioSolicitante", ticket.IdUsuarioSolicitante);
                comando.Parameters.AddWithValue("@IdEstado", ticket.IdEstado);
                comando.Parameters.AddWithValue("@Categoria", ticket.Categoria);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        // Consultar los tickets de un usuario específico
        public List<Ticket> ObtenerPorUsuario(int idUsuario)
        {
            // Crear una lista para almacenar los tickets obtenidos
            List<Ticket> lista = new List<Ticket>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener los tickets de un usuario específico
                string consulta = @"
                    SELECT T.Id, T.Titulo, T.Descripcion,
                           T.IdUsuarioSolicitante, T.IdUsuarioAsignado,
                           T.IdEstado, E.Nombre AS NombreEstado,
                           T.FechaCreacion, T.FechaActualizacion,
                           T.Categoria
                    FROM Tickets T
                    INNER JOIN EstadosTicket E ON T.IdEstado = E.Id
                    WHERE T.IdUsuarioSolicitante = @IdUsuario
                    ORDER BY T.FechaCreacion DESC";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);
                // Agregar el parámetro necesario para la consulta SQL
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

                conexion.Open();
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader lector = comando.ExecuteReader();
                // Recorrer los resultados y mapearlos a objetos Ticket
                while (lector.Read())
                {
                    // Mapear los datos del lector a un objeto Ticket y agregarlo a la lista
                    lista.Add(MapearTicket(lector));
                }
            }

            return lista;
        }

        // Permite convertir los datos de SQL Server en un objeto Ticket 
        private Ticket MapearTicket(SqlDataReader lector)
        {
            // Crear un nuevo objeto Ticket y asignar los valores obtenidos del lector
            return new Ticket
            {
                Id = Convert.ToInt32(lector["Id"]),
                Titulo = lector["Titulo"].ToString(),
                Descripcion = lector["Descripcion"].ToString(),
                IdUsuarioSolicitante = Convert.ToInt32(
                    lector["IdUsuarioSolicitante"]),
                // Verificar si el valor de IdUsuarioAsignado es nulo y asignar null si es así
                IdUsuarioAsignado = lector["IdUsuarioAsignado"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(lector["IdUsuarioAsignado"]),
                IdEstado = Convert.ToInt32(lector["IdEstado"]),
                NombreEstado = lector["NombreEstado"].ToString(),
                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                FechaActualizacion = Convert.ToDateTime(
                    lector["FechaActualizacion"]),
                Categoria = lector["Categoria"].ToString()
            };
        }




        // Consultar un ticket específico
        public Ticket ObtenerPorId(int id)
        {
            // Inicializar un objeto Ticket como nulo
            // para almacenar el ticket obtenido de la base de datos
            Ticket ticket = null;
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener un ticket específico por su Id
                string consulta = @"
                SELECT T.Id, T.Titulo, T.Descripcion,
                       T.IdUsuarioSolicitante, T.IdUsuarioAsignado,
                       T.IdEstado, E.Nombre AS NombreEstado,
                       T.FechaCreacion, T.FechaActualizacion,
                       T.Categoria
                FROM Tickets T
                INNER JOIN EstadosTicket E ON T.IdEstado = E.Id
                WHERE T.Id = @Id";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);
                // Agregar el parámetro necesario para la consulta SQL
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader lector = comando.ExecuteReader();
                // Verificar si se obtuvo un resultado y mapearlo a un objeto Ticket
                if (lector.Read())
                {
                    // Mapear los datos del lector a un objeto Ticket
                    ticket = new Ticket
                    {
                        Id = Convert.ToInt32(lector["Id"]),
                        Titulo = lector["Titulo"].ToString(),
                        Descripcion = lector["Descripcion"].ToString(),
                        IdUsuarioSolicitante =
                            Convert.ToInt32(lector["IdUsuarioSolicitante"]),
                        IdUsuarioAsignado = lector["IdUsuarioAsignado"] == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(lector["IdUsuarioAsignado"]),
                        IdEstado = Convert.ToInt32(lector["IdEstado"]),
                        NombreEstado = lector["NombreEstado"].ToString(),
                        FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                        FechaActualizacion =
                            Convert.ToDateTime(lector["FechaActualizacion"]),
                        Categoria = lector["Categoria"].ToString()
                    };
                }
            }
            // Devolver el ticket obtenido de la base de datos (o null si no se encontró)
            return ticket;
        }

    }
}