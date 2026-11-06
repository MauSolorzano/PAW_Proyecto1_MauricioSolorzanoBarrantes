using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del ticket es obligatorio.")]
        public int IdTicket { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El texto del comentario es obligatorio.")]
        [StringLength(2000, ErrorMessage = "El comentario no puede superar los 2000 caracteres.")]
        public string Texto { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}