using System;
using System.ComponentModel.DataAnnotations;

namespace BlueTravel.Models
{
    public class Resena
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [StringLength(100, ErrorMessage = "El usuario no puede exceder 100 caracteres")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        [Display(Name = "Comentario")]
        public string Comentario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La calificación es obligatoria")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5 estrellas")]
        [Display(Name = "Calificación (1-5)")]
        public int Calificacion { get; set; } // 1 a 5 estrellas

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }
    }
}