using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; }

        // Si es requerido en SQL (NOT NULL), asegúrate de validarlo aquí si aplica
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El usuario solicitante es obligatorio.")]
        public int IdUsuarioSolicitante { get; set; }

        public int? IdUsuarioAsignado { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(80, ErrorMessage = "La categoría no puede superar los 80 caracteres.")] // Sincronizado con NVARCHAR(80) de SQL
        public string Categoria { get; set; }
    }
}