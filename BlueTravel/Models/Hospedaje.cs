using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueTravel.Models
{
    public class Hospedaje
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Hospedaje")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio por noche es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio por Noche")]
        public decimal PrecioPorNoche { get; set; }

        [Required(ErrorMessage = "La capacidad máxima es obligatoria")]
        [Range(1, 50, ErrorMessage = "La capacidad debe estar entre 1 y 50 personas")]
        [Display(Name = "Capacidad Máxima")]
        public int CapacidadMaxima { get; set; } = 2;

        [Required(ErrorMessage = "Las personas incluidas en precio es obligatorio")]
        [Range(1, 50, ErrorMessage = "Debe estar entre 1 y 50 personas")]
        [Display(Name = "Personas Incluidas en Precio")]
        public int PersonasIncluidasEnPrecio { get; set; } = 2;

        [Required(ErrorMessage = "El cargo por persona extra es obligatorio")]
        [Range(0, 999999.99, ErrorMessage = "El cargo debe estar entre ₡0 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Cargo por Persona Extra")]
        public decimal CargoPorPersonaExtra { get; set; } = 0;

        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Hospedaje")]
        public string? TipoHospedaje { get; set; }

        [Display(Name = "Permite Niños")]
        public bool PermiteNinos { get; set; } = true;

        [Display(Name = "Permite Mascotas")]
        public bool PermiteMascotas { get; set; } = false;

        [Range(0, 23, ErrorMessage = "La hora debe estar entre 0 y 23")]
        [Display(Name = "Hora de Check-in")]
        public int HoraCheckIn { get; set; } = 15;

        [Range(0, 23, ErrorMessage = "La hora debe estar entre 0 y 23")]
        [Display(Name = "Hora de Check-out")]
        public int HoraCheckOut { get; set; } = 11;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(500)]
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        // Propiedad calculada para mostrar información de capacidad
        [NotMapped]
        public string InfoCapacidad => $"{PersonasIncluidasEnPrecio} persona(s) incluida(s), máx. {CapacidadMaxima}";
    }
}