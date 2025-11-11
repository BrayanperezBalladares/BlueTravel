using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueTravel.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del tour es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Tour")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio (Adultos)")]
        public decimal Precio { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio Niños (0-12 años)")]
        public decimal? PrecioNino { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre ₡0.01 y ₡999,999.99")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio Seniors (65+)")]
        public decimal? PrecioSenior { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Range(1, 365, ErrorMessage = "La duración debe estar entre 1 y 365 días")]
        [Display(Name = "Duración (días)")]
        public int Duracion { get; set; }

        [Required(ErrorMessage = "La fecha disponible es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Disponible")]
        public DateTime FechaDisponible { get; set; }

        [Required(ErrorMessage = "Los cupos totales son obligatorios")]
        [Range(1, 1000, ErrorMessage = "Los cupos deben estar entre 1 y 1000")]
        [Display(Name = "Cupos Totales")]
        public int CuposTotales { get; set; } = 20;

        [Display(Name = "Cupo Máximo (Alias)")]
        [NotMapped]
        public int CupoMaximo
        {
            get => CuposTotales;
            set => CuposTotales = value;
        }

        [Range(0, 1000, ErrorMessage = "Los cupos reservados deben estar entre 0 y 1000")]
        [Display(Name = "Cupos Reservados")]
        public int CuposReservados { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0% y 100%")]
        [Display(Name = "Descuento de Grupo (%)")]
        public decimal DescuentoGrupo { get; set; } = 0;

        [StringLength(50, ErrorMessage = "El nivel no puede exceder 50 caracteres")]
        [Display(Name = "Nivel de Dificultad")]
        public string? NivelDificultad { get; set; }

        [Range(0, 120, ErrorMessage = "La edad mínima debe estar entre 0 y 120")]
        [Display(Name = "Edad Mínima")]
        public int EdadMinima { get; set; } = 0;

        [Range(0, 120, ErrorMessage = "La edad máxima debe estar entre 0 y 120")]
        [Display(Name = "Edad Máxima")]
        public int? EdadMaxima { get; set; }

        [Display(Name = "Requiere Confirmación")]
        public bool RequiereConfirmacion { get; set; } = false;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(500)]
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public int CuposDisponibles => CuposTotales - CuposReservados;

        [NotMapped]
        public string EstadoDisponibilidad
        {
            get
            {
                if (CuposDisponibles <= 0)
                    return "Agotado";
                else if (CuposDisponibles <= 5)
                    return "Pocos cupos";
                else
                    return "Disponible";
            }
        }
    }
}
