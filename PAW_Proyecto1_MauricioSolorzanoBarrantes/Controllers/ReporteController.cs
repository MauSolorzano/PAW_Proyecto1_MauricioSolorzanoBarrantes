using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Data;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Filtros;



namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Controllers
{
    [Autenticacion(Roles = "Administrador")]
    public class ReporteController : Controller
    {
        private TicketData ticketData = new TicketData();

        // Mostrar reporte de tickets por estado
        public ActionResult Index()
        {
            var reporte = ticketData.ObtenerReportePorEstado();

            return View(reporte);
        }
    }
}