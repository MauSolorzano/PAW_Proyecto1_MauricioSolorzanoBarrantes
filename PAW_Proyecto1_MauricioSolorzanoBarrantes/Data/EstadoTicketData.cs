using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Data
{
    public class EstadoTicketData
    {
        public List<EstadoTicket> ObtenerTodos()
        {
            // Crear una lista para almacenar los estados de ticket obtenidos
            List<EstadoTicket> lista = new List<EstadoTicket>();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para obtener todos los estados de ticket de la tabla EstadosTicket
                string consulta = "SELECT Id, Nombre FROM EstadosTicket";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);

                conexion.Open();
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader lector = comando.ExecuteReader();
                // Recorrer los resultados y mapearlos a objetos EstadoTicket
                while (lector.Read())
                {
                    // Crear un nuevo objeto EstadoTicket y agregarlo a la lista
                    lista.Add(new EstadoTicket
                    {
                        Id = (int)lector["Id"],
                        Nombre = lector["Nombre"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}