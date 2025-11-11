using System.ComponentModel.DataAnnotations;

namespace BlueTravel.Models
{
    public class Restaurante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Restaurante")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de comida es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de comida no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Comida")]
        public string TipoComida { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(500)]
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        [StringLength(100, ErrorMessage = "La especialidad no puede exceder 100 caracteres")]
        [Display(Name = "Especialidad")]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio promedio es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio Promedio")]
        public decimal PrecioPromedio { get; set; }
    }
}