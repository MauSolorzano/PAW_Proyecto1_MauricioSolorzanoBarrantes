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
            
            int idUsuario = Convert.ToInt32(Session["UsuarioId"]);

            var tickets = datos.ObtenerPorUsuario(idUsuario);

            return View(tickets);
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
    }
}