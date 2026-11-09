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
                // Definir la consulta SQL para obtener todos los tickets junto con el nombre del estado
                string consulta = @"
                SELECT T.Id, T.Titulo, T.Descripcion,
                       T.IdUsuarioSolicitante, T.IdUsuarioAsignado,
                       T.IdEstado, E.Nombre AS NombreEstado,
                       T.FechaCreacion, T.FechaActualizacion,
                       T.Categoria
                FROM Tickets T
                INNER JOIN EstadosTicket E ON T.IdEstado = E.Id
                ORDER BY T.FechaCreacion DESC";
                // Crear un comando SQL para ejecutar la consulta
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    // Ejecutar la consulta y obtener un lector de datos
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        // Recorrer los resultados y mapearlos a objetos Ticket
                        while (lector.Read())
                        {
                            lista.Add(MapearTicket(lector));
                        }
                    }
                }
            }
            // Devolver la lista de tickets obtenidos de la base de datos
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
        //Obtener ticket por ID
        public Ticket ObtenerPorId(int id)
        {
            //Declarar una variable para almacenar el ticket obtenido de la base de datos
            Ticket ticket = null;
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener un ticket específico por su ID
                string consulta = @"
                    SELECT T.Id, T.Titulo, T.Descripcion,
                           T.IdUsuarioSolicitante, T.IdUsuarioAsignado,
                           T.IdEstado, E.Nombre AS NombreEstado,
                           T.FechaCreacion, T.FechaActualizacion,
                           T.Categoria,
                           U.Nombre AS NombreUsuarioAsignado
                    FROM Tickets T
                    INNER JOIN EstadosTicket E ON T.IdEstado = E.Id
                    LEFT JOIN Usuarios U ON T.IdUsuarioAsignado = U.Id
                    WHERE T.Id = @Id";
                // Crear un comando SQL para ejecutar la consulta
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    conexion.Open();
                    // Ejecutar la consulta y obtener un lector de datos
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        //
                        if (lector.Read())
                        {
                            ticket = new Ticket
                            {
                                Id = Convert.ToInt32(lector["Id"]),
                                Titulo = lector["Titulo"].ToString(),
                                Descripcion = lector["Descripcion"].ToString(),
                                IdUsuarioSolicitante = Convert.ToInt32(lector["IdUsuarioSolicitante"]),
                                IdUsuarioAsignado = lector["IdUsuarioAsignado"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(lector["IdUsuarioAsignado"]),
                                IdEstado = Convert.ToInt32(lector["IdEstado"]),
                                NombreEstado = lector["NombreEstado"].ToString(),
                                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                                FechaActualizacion = Convert.ToDateTime(lector["FechaActualizacion"]),
                                Categoria = lector["Categoria"].ToString(),
                                NombreUsuarioAsignado = lector["NombreUsuarioAsignado"] == DBNull.Value
                                    ? null
                                    : lector["NombreUsuarioAsignado"].ToString()
                            };
                        }
                    }
                }
            }

            return ticket;
        }

        // Asignar un usuario a un ticket específico
        public void AsignarUsuario(int idTicket, int idUsuario)
        {
            // Establecer la conexión con la base de datos utilizando la clase Conexion 

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para actualizar el ticket con el IdUsuarioAsignado
                string consulta = @"
                UPDATE Tickets
                SET IdUsuarioAsignado = @IdUsuario,
                    FechaActualizacion = GETDATE()
                WHERE Id = @IdTicket";
                // Crear un comando SQL para ejecutar la consulta
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdTicket", idTicket);
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }
        //Metodo para cambiar el estado del ticket
        public void CambiarEstado(int idTicket, int idEstado)
        {
            //Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para actualizar el estado del ticket
                string consulta = @"
                UPDATE Tickets
                SET IdEstado = @IdEstado,
                    FechaActualizacion = GETDATE()
                WHERE Id = @IdTicket";
                // Crear un comando SQL para ejecutar la consulta
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdTicket", idTicket);
                    comando.Parameters.AddWithValue("@IdEstado", idEstado);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        // Método para buscar tickets según un término de búsqueda, el ID del usuario y si es administrador
        public List<Ticket> Buscar(string busqueda, int idUsuario, bool esAdministrador)
        {
            // Crear una lista para almacenar los tickets obtenidos
            List<Ticket> lista = new List<Ticket>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para buscar tickets según el término de búsqueda y el ID del usuario
                string consulta = @"
                    SELECT T.Id, T.Titulo, T.Descripcion,
                           T.IdUsuarioSolicitante, T.IdUsuarioAsignado,
                           T.IdEstado, E.Nombre AS NombreEstado,
                           T.FechaCreacion, T.FechaActualizacion,
                           T.Categoria
                    FROM Tickets T
                    INNER JOIN EstadosTicket E ON T.IdEstado = E.Id
                    WHERE (
                        CAST(T.Id AS NVARCHAR(20)) LIKE @Busqueda
                        OR T.Titulo LIKE @Busqueda
                        OR E.Nombre LIKE @Busqueda
                    )";
                // Si el usuario no es administrador, agregar una condición para filtrar por su ID de usuario
                if (!esAdministrador)
                {
                    consulta += " AND T.IdUsuarioSolicitante = @IdUsuario";
                }
                // Agregar una cláusula ORDER BY para ordenar los resultados por fecha de creación en orden descendente
                consulta += " ORDER BY T.FechaCreacion DESC";
                // Crear un comando SQL para ejecutar la consulta
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    // Agregar los parámetros necesarios para la consulta SQL
                    comando.Parameters.AddWithValue(
                        "@Busqueda",
                        "%" + (busqueda ?? "").Trim() + "%"
                    );
                    // Si el usuario no es administrador, agregar el parámetro del ID de usuario
                    if (!esAdministrador)
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    }

                    conexion.Open();
                    // Ejecutar la consulta y obtener un lector de datos
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(MapearTicket(lector));
                        }
                    }
                }
            }

            return lista;
        }
    }
}