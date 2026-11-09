using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Models
{
    public class EstadoTicket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, ErrorMessage = "El nombre no puede superar los 20 caracteres.")]
        public string Nombre { get; set; }
    }
}