using PAW_Proyecto1_MauricioSolorzanoBarrantes.Data;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Filtros;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Controllers
{
    [Autenticacion]
    public class TicketController : Controller
    {

        // Instancia de la clase TicketData para acceder a los métodos de datos relacionados con los tickets.
        private TicketData datos = new TicketData();

        // Mostrar los tickets del usuario autenticado
        public ActionResult Index()
        {
            // Obtener el rol del usuario autenticado desde la sesión
            string rol = Session["UsuarioRol"].ToString();
            // Verificar si el usuario es un administrador
            if (rol == "Administrador")
            {
                var tickets = datos.ObtenerTodos();
                return View(tickets);
            }
            // Si el usuario no es un administrador, obtener solo los tickets del usuario autenticado
            else
            {
                // Obtener el ID del usuario autenticado desde la sesión
                int idUsuario = Convert.ToInt32(Session["UsuarioId"]);
                // Obtener los tickets del usuario autenticado utilizando la clase TicketData
                var tickets = datos.ObtenerPorUsuario(idUsuario);
                return View(tickets);
            }
        }
        // Mostrar formulario para crear un ticket
        [HttpGet]
        public ActionResult Crear()
        {
            return View();
        }

        // Registrar un nuevo ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Ticket ticket)
        {
            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrWhiteSpace(ticket.Titulo) ||
                string.IsNullOrWhiteSpace(ticket.Descripcion) ||
                string.IsNullOrWhiteSpace(ticket.Categoria))
            {
                // Mostrar mensaje de error y devolver la vista con el ticket ingresado
                ViewBag.Mensaje = "Todos los campos son obligatorios.";
                return View(ticket);
            }
            // Asignar el ID del usuario solicitante desde la sesión
            ticket.IdUsuarioSolicitante =
                Convert.ToInt32(Session["UsuarioId"]);

            // Todo ticket nuevo inicia en estado Abierto (Id = 1)
            ticket.IdEstado = 1;
            //Crear el ticket en la base de datos utilizando la clase TicketData
            datos.Crear(ticket);
            // Redirigir a la lista de tickets con un mensaje de éxito
            TempData["Mensaje"] = "Ticket creado correctamente.";
            // Redirigir a la lista de tickets
            return RedirectToAction("Index");
        }

        // Mostrar detalles de un ticket específico
        public ActionResult Detalle(int id)
        {
            // Obtener el ticket por su ID utilizando la clase TicketData
            var ticket = datos.ObtenerPorId(id);
            // Verificar si el ticket existe, si no, devolver un error 404
            if (ticket == null)
            {
                // Devolver un error 404 si el ticket no se encuentra
                return HttpNotFound();
            }
            // Obtener el ID del usuario autenticado y su rol desde la sesión
            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            // Obtener el rol del usuario desde la sesión
            string rol = Session["UsuarioRol"].ToString();
            // Verificar si el usuario tiene permiso para ver el ticket
            if (rol != "Administrador" &&
                ticket.IdUsuarioSolicitante != idUsuario)
            {
                // Devolver un error 403 si el usuario no tiene permiso para ver el ticket
                return new HttpStatusCodeResult(403);
            }
            // Si el usuario es administrador, obtener la lista de usuarios activos para asignar tickets
            if (rol == "Administrador")
            {
                // Crear una instancia de UsuarioData y EstadoTicketData para obtener la lista de usuarios y estados de tickets
                UsuarioData usuarioDatos = new UsuarioData();
                EstadoTicketData estadoDatos = new EstadoTicketData();
                // Obtener la lista de usuarios activos y estados de tickets para mostrarlos en la vista
                ViewBag.Usuarios = usuarioDatos.ObtenerUsuarios();
                ViewBag.Estados = estadoDatos.ObtenerTodos();
            }
            // Devolver la vista con el ticket obtenido de la base de datos
            return View(ticket);
        }

        //Asignar un ticket a un usuario específico
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autenticacion(RolPermitido = "Administrador")]
        public ActionResult Asignar(int idTicket, int idUsuario)
        {
            // Validar que los IDs del ticket y del usuario sean válidos (mayores a cero)
            if (idTicket <= 0 || idUsuario <= 0)
            {
                return new HttpStatusCodeResult(400);
            }
            // Crear una instancia de UsuarioData para buscar el usuario por su ID
            UsuarioData usuarioDatos = new UsuarioData();
            // Buscar el usuario por su ID utilizando la clase UsuarioData
            var usuario = usuarioDatos.ObtenerUsuarios()
                .Find(u => u.Id == idUsuario);
            // Validar que el usuario exista, si no, devolver un error 400
            if (usuario == null)
            {
                return new HttpStatusCodeResult(400);
            }
            // Obtener el ticket por su ID utilizando la clase TicketData
            var ticket = datos.ObtenerPorId(idTicket);
            //  Validar que el ticket exista, si no, devolver un error 404
            if (ticket == null)
            {
                return HttpNotFound();
            }
            // Asignar el ticket al usuario utilizando la clase TicketData
            datos.AsignarUsuario(idTicket, idUsuario);
            //  Guardar un mensaje de éxito en TempData para mostrarlo en la vista
            TempData["Mensaje"] = "Ticket asignado correctamente.";
            // Redirigir a la vista de detalles del ticket asignado
            return RedirectToAction("Detalle", new { id = idTicket });
        }
        // Cambiar el estado de un ticket específico
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autenticacion(RolPermitido = "Administrador")]
        public ActionResult CambiarEstado(int idTicket, int idEstado)
        {
            // Validar que los IDs del ticket y del estado sean válidos (mayores a cero)
            if (idTicket <= 0 || idEstado <= 0)
            {
                return new HttpStatusCodeResult(400);
            }
            // Crear una instancia de EstadoTicketDatos para obtener la lista de estados de tickets
            EstadoTicketData estadoDatos = new EstadoTicketData();
            // Obtener todos los estados de tickets utilizando la clase EstadoTicketData
            var estados = estadoDatos.ObtenerTodos();
            // Validar que el estado especificado exista en la lista de estados
            bool estadoExiste = estados.Exists(e => e.Id == idEstado);
            // Si el estado no existe, devolver un error 400
            if (!estadoExiste)
            {
                return new HttpStatusCodeResult(400);
            }
            // Obtener el ticket por su ID utilizando la clase TicketData
            var ticket = datos.ObtenerPorId(idTicket);
            // Validar que el ticket exista, si no, devolver un error 404
            if (ticket == null)
            {
                return HttpNotFound();
            }
            // Cambiar el estado del ticket utilizando la clase TicketData
            datos.CambiarEstado(idTicket, idEstado);
            // Guardar un mensaje de éxito en TempData para mostrarlo en la vista
            TempData["Mensaje"] = "Estado del ticket actualizado correctamente.";
            // Redirigir a la vista de detalles del ticket con el nuevo estado
            return RedirectToAction("Detalle", new { id = idTicket });
        }

        //Método para buscar tickets según un término de búsqueda
        [HttpGet]
        public ActionResult Buscar(string busqueda)
        {
            // Obtener el ID del usuario autenticado y su rol desde la sesión
            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            // Obtener el rol del usuario desde la sesión
            string rol = Session["UsuarioRol"].ToString();
            // Verificar si el usuario es un administrador
            bool esAdministrador = rol == "Administrador";
            // Llamar al método Buscar de la clase TicketData para obtener los tickets que coinciden con el término de búsqueda
            var tickets = datos.Buscar(
                busqueda,
                idUsuario,
                esAdministrador
            );
            // Devolver los tickets encontrados en formato JSON para que puedan ser procesados por el cliente
            return Json(tickets, JsonRequestBehavior.AllowGet);
        }
    }
}