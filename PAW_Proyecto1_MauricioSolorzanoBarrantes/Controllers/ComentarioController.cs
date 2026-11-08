using PAW_Proyecto1_MauricioSolorzanoBarrantes.Data;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Filtros;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Controllers
{

    [Autenticacion]
    public class ComentarioController : Controller
    {
        private ComentarioData datos = new ComentarioData();

        // Verificar si el usuario puede acceder al ticket
        private bool PuedeAcceder(int idTicket)
        {
            // Obtener el ID del usuario 
            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            // Obtener el rol del usuario desde la sesión
            string rol = Session["UsuarioRol"].ToString();
            // Establecer la conexión con la base de datos utilizando la clase Conexion
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                // Definir la consulta SQL para verificar si el usuario puede acceder al ticket
                string consulta = @"
                    SELECT COUNT(*)
                    FROM Tickets
                    WHERE Id = @IdTicket
                    AND (IdUsuarioSolicitante = @IdUsuario
                         OR @Rol = 'Administrador')";
                // Crear un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@IdTicket", idTicket);
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                comando.Parameters.AddWithValue("@Rol", rol);

                conexion.Open();
                // Ejecutar la consulta y obtener la cantidad de registros que cumplen con la condición
                int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                // Devolver true si el usuario puede acceder al ticket, false en caso contrario
                return cantidad > 0;
            }
        }

        // Consultar los comentarios mediante AJAX
        [HttpGet]
        public ActionResult Obtener(int idTicket)
        {
            // Verificar si el usuario tiene permiso para acceder al ticket
            if (!PuedeAcceder(idTicket))
            {
                // Devolver un resultado de estado HTTP 403 (Prohibido) si el usuario no tiene permiso
                return new HttpStatusCodeResult(403);
            }
            // Obtener los comentarios del ticket utilizando la clase ComentarioData
            var comentarios = datos.ObtenerPorTicket(idTicket);
            // Devolver los comentarios en formato JSON para ser consumidos por la vista mediante AJAX
            return Json(comentarios, JsonRequestBehavior.AllowGet);
        }

        // Registrar comentario mediante AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(int idTicket, string texto)
        {
            // Verificar si el usuario tiene permiso para acceder al ticket
            if (!PuedeAcceder(idTicket))
            {
                // Devolver un resultado JSON indicando que el usuario no tiene permiso para comentar en el ticket
                return Json(new
                {
                    ok = false,
                    mensaje = "No tienes permiso para comentar en este ticket."
                });
            }
            // Validar que el comentario no esté vacío y no supere los 2000 caracteres
            if (string.IsNullOrWhiteSpace(texto))
            {
                // Devolver un resultado JSON indicando que el comentario no puede estar vacío
                return Json(new
                {
                    ok = false,
                    mensaje = "El comentario no puede estar vacío."
                });
            }
            // Validar que el comentario no supere los 2000 caracteres
            if (texto.Trim().Length > 2000)
            {
                // Devolver un resultado JSON indicando que el comentario no puede superar los 2000 caracteres
                return Json(new
                {
                    ok = false,
                    mensaje = "El comentario no puede superar los 2000 caracteres."
                });
            }
            // Obtener el ID del usuario desde la sesión
            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            // Crear un objeto Comentario con los datos proporcionados
            var comentario = new Comentario
            {
                IdTicket = idTicket,
                IdUsuario = idUsuario,
                Texto = texto.Trim()
            };
            // Registrar el comentario en la base de datos utilizando la clase ComentarioData
            datos.Crear(comentario);
            // Devolver un resultado JSON indicando que el comentario se registró correctamente
            return Json(new
            {
                ok = true,
                mensaje = "Comentario registrado correctamente."
            });
        }
    }
}