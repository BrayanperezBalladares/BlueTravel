using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueTravel.Models
{
    /// <summary>
    /// Modelo profesional de Pago con trazabilidad y seguridad
    /// </summary>
    public class Pago
    {
        public int Id { get; set; }

        // 👇 RELACIÓN CON RESERVA (CRÍTICO)
        [Required]
        [Display(Name = "ID de Reserva")]
        public int ReservaId { get; set; }
        
        [ForeignKey("ReservaId")]
        public Reserva? Reserva { get; set; }

        // 👇 RELACIÓN CON USUARIO (MEJOR QUE STRING)
        [Required]
        [Display(Name = "Usuario ID")]
        public string UsuarioId { get; set; } = string.Empty;

        // 👇 MÉTODO DE PAGO CON ENUM (MEJOR QUE STRING LIBRE)
        [Required]
        [Display(Name = "Método de Pago")]
        public MetodoPago Metodo { get; set; }

        // 👇 MONTOS DETALLADOS
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto Base")]
        public decimal MontoBase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Impuestos (13%)")]
        public decimal Impuestos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cargos Adicionales")]
        public decimal CargosAdicionales { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Descuentos")]
        public decimal Descuentos { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Pagado")]
        public decimal TotalPagado { get; set; }

        // 👇 INFORMACIÓN DE TRANSACCIÓN (TRAZABILIDAD)
        [StringLength(100)]
        [Display(Name = "ID de Transacción Externa")]
        public string? TransaccionExternaId { get; set; } // ID de Stripe, PayPal, SINPE

        [StringLength(500)]
        [Display(Name = "Referencia del Banco")]
        public string? ReferenciaBancaria { get; set; }

        // 👇 SEGURIDAD Y AUDITORÍA
        [Required]
        [Display(Name = "Estado del Pago")]
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Aprobación")]
        public DateTime? FechaAprobacion { get; set; }

        [Display(Name = "Fecha de Cancelación")]
        public DateTime? FechaCancelacion { get; set; }

        [StringLength(500)]
        [Display(Name = "Notas Internas")]
        public string? NotasInternas { get; set; }

        // 👇 INFORMACIÓN PARA REEMBOLSOS
        [Display(Name = "Es Reembolso")]
        public bool EsReembolso { get; set; }

        [Display(Name = "Pago Original (si es reembolso)")]
        public int? PagoOriginalId { get; set; }

        // 👇 DATOS ENMASCARADOS DE TARJETA (NUNCA GUARDAR COMPLETOS)
        [StringLength(4)]
        [Display(Name = "Últimos 4 Dígitos")]
        public string? UltimosDigitosTarjeta { get; set; } // Solo últimos 4

        [StringLength(20)]
        [Display(Name = "Marca de Tarjeta")]
        public string? MarcaTarjeta { get; set; } // Visa, Mastercard, etc.

        // 👇 PROPIEDAD CALCULADA
        [NotMapped]
        public bool EstaPagado => Estado == EstadoPago.Aprobado;

        [NotMapped]
        public string EstadoBadgeClass => Estado switch
        {
            EstadoPago.Aprobado => "bg-success",
            EstadoPago.Pendiente => "bg-warning text-dark",
            EstadoPago.Procesando => "bg-info",
            EstadoPago.Rechazado => "bg-danger",
            EstadoPago.Cancelado => "bg-secondary",
            EstadoPago.Reembolsado => "bg-dark",
            EstadoPago.EnDisputa => "bg-danger",
            _ => "bg-secondary"
        };
    }

    /// <summary>
    /// Métodos de pago permitidos en Costa Rica
    /// </summary>
    public enum MetodoPago
    {
        [Display(Name = "Tarjeta de Crédito")]
        TarjetaCredito = 1,

        [Display(Name = "Tarjeta de Débito")]
        TarjetaDebito = 2,

        [Display(Name = "SINPE Móvil")]
        SinpeMovil = 3,

        [Display(Name = "Transferencia Bancaria")]
        TransferenciaBancaria = 4,

        [Display(Name = "PayPal")]
        PayPal = 5,

        [Display(Name = "Efectivo (Pago en Oficina)")]
        Efectivo = 6,

        [Display(Name = "Depósito Bancario")]
        DepositoBancario = 7
    }

    /// <summary>
    /// Estados del ciclo de vida del pago
    /// </summary>
    public enum EstadoPago
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,

        [Display(Name = "Procesando")]
        Procesando = 2,

        [Display(Name = "Aprobado")]
        Aprobado = 3,

        [Display(Name = "Rechazado")]
        Rechazado = 4,

        [Display(Name = "Cancelado")]
        Cancelado = 5,

        [Display(Name = "Reembolsado")]
        Reembolsado = 6,

        [Display(Name = "En Disputa")]
        EnDisputa = 7
    }
}