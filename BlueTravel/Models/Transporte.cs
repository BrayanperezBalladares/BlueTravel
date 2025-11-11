using System.ComponentModel.DataAnnotations;

namespace BlueTravel.Models
{
    public class Transporte
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Transporte")]
        public string Tipo { get; set; } = string.Empty; // Ej: Shuttle, Renta de auto, Taxi

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [StringLength(100, ErrorMessage = "La empresa no puede exceder 100 caracteres")]
        [Display(Name = "Empresa")]
        public string Empresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(500)]
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }
    }
}