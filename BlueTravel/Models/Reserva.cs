using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueTravel.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        // Relación con el usuario que hace la reserva
        [StringLength(450)]
        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; } = string.Empty;

        // Tipo de reserva (Hospedaje o Tour)
        [Required(ErrorMessage = "El tipo de reserva es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Reserva")]
        public string TipoReserva { get; set; } = string.Empty;

        // ID del elemento reservado
        [Required(ErrorMessage = "El ID del item es obligatorio")]
        [Display(Name = "Item Reservado")]
        public int ItemId { get; set; }

        // Nombre del item reservado
        [Required(ErrorMessage = "El nombre del item es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Nombre del Item")]
        public string ItemNombre { get; set; } = string.Empty;

        // Fechas
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio / Check-in")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin / Check-out")]
        public DateTime FechaFin { get; set; }

        // Cantidad de personas - MEJORADO
        [Required(ErrorMessage = "La cantidad de adultos es obligatoria")]
        [Range(1, 50, ErrorMessage = "Debe haber entre 1 y 50 adultos")]
        [Display(Name = "Cantidad de Adultos")]
        public int CantidadAdultos { get; set; } = 1;

        [Range(0, 50, ErrorMessage = "La cantidad de niños debe estar entre 0 y 50")]
        [Display(Name = "Cantidad de Niños (0-12 años)")]
        public int CantidadNinos { get; set; } = 0;

        [Range(0, 50, ErrorMessage = "La cantidad de seniors debe estar entre 0 y 50")]
        [Display(Name = "Cantidad de Seniors (65+)")]
        public int CantidadSeniors { get; set; } = 0;

        // Precio desglosado
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Base")]
        public decimal PrecioBase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cargo Personas Extra")]
        public decimal CargoPersonasExtra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Descuento Aplicado")]
        public decimal DescuentoAplicado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Total")]
        public decimal PrecioTotal { get; set; }

        // Estado de la reserva
        [StringLength(50)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada, Completada

        [StringLength(100)]
        [Display(Name = "Motivo de Rechazo/Cancelación")]
        public string? MotivoRechazo { get; set; }

        // Información adicional
        [StringLength(1000)]
        [Display(Name = "Comentarios Adicionales")]
        public string? Comentarios { get; set; }

        // Confirmación para tours
        [Display(Name = "Requiere Confirmación")]
        public bool RequiereConfirmacion { get; set; } = false;

        [Display(Name = "Confirmado Por")]
        [StringLength(450)]
        public string? ConfirmadoPor { get; set; }

        [Display(Name = "Fecha de Confirmación")]
        public DateTime? FechaConfirmacion { get; set; }

        // Auditoría
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        // ?? NUEVAS PROPIEDADES - RELACIÓN CON PAGO
        [Display(Name = "Pago Asociado")]
        public int? PagoId { get; set; }
        
        [ForeignKey("PagoId")]
        public Pago? Pago { get; set; }

        // ?? TRACKING DE CAMBIOS DE ESTADO
        [StringLength(50)]
        [Display(Name = "Estado Anterior")]
        public string? EstadoAnterior { get; set; }

        [Display(Name = "Fecha Cambio Estado")]
        public DateTime? FechaCambioEstado { get; set; }

        [StringLength(450)]
        [Display(Name = "Modificado Por")]
        public string? ModificadoPor { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public int CantidadPersonas => CantidadAdultos + CantidadNinos + CantidadSeniors;

        [NotMapped]
        public int DiasEstancia => (FechaFin - FechaInicio).Days;

        [NotMapped]
        public string EstadoBadgeClass => Estado switch
        {
            "Confirmada" => "bg-success",
            "Pendiente" => "bg-warning text-dark",
            "Cancelada" => "bg-danger",
            "Completada" => "bg-info",
            _ => "bg-secondary"
        };

        [NotMapped]
        public string DesglosePrecio => $"Base: {PrecioBase:C} | Extra: {CargoPersonasExtra:C} | Desc: -{DescuentoAplicado:C} | Total: {PrecioTotal:C}";

        [NotMapped]
        public string DetallePersonas
        {
            get
            {
                var partes = new List<string>();
                if (CantidadAdultos > 0) partes.Add($"{CantidadAdultos} adulto(s)");
                if (CantidadNinos > 0) partes.Add($"{CantidadNinos} niño(s)");
                if (CantidadSeniors > 0) partes.Add($"{CantidadSeniors} senior(s)");
                return string.Join(", ", partes);
            }
        }

        [NotMapped]
        public bool EstaPagada => Pago?.EstaPagado ?? false;

        [NotMapped]
        public string EstadoPagoTexto => EstaPagada ? "Pagado" : "Pendiente de Pago";

        [NotMapped]
        public int DiasParaInicio => (FechaInicio - DateTime.Today).Days;

        [NotMapped]
        public bool EsProxima => DiasParaInicio >= 0 && DiasParaInicio <= 7;
    }
}
