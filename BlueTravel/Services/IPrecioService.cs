using BlueTravel.Models;

namespace BlueTravel.Services
{
    /// <summary>
    /// Servicio centralizado para cálculo de precios
    /// Aplica estrategia de precios según tipo de servicio
    /// </summary>
    public interface IPrecioService
    {
        /// <summary>
        /// Calcula el precio total de una reserva de hospedaje
        /// </summary>
        Task<ResultadoCalculo> CalcularPrecioHospedaje(
            Hospedaje hospedaje, 
            DateTime fechaInicio, 
            DateTime fechaFin,
            int cantidadAdultos,
            int cantidadNinos,
            int cantidadSeniors);

        /// <summary>
        /// Calcula el precio total de una reserva de tour
        /// </summary>
        Task<ResultadoCalculo> CalcularPrecioTour(
            Tour tour,
            int cantidadAdultos,
            int cantidadNinos,
            int cantidadSeniors);

        /// <summary>
        /// Aplica descuentos promocionales si están disponibles
        /// </summary>
        Task<decimal> AplicarDescuentosPromocionales(
            string tipoReserva, 
            int itemId, 
            decimal precioBase);

        /// <summary>
        /// Calcula impuestos (13% IVA en Costa Rica)
        /// </summary>
        decimal CalcularImpuestos(decimal precioBase);
    }

    public class ResultadoCalculo
    {
        public decimal PrecioBase { get; set; }
        public decimal CargoPersonasExtra { get; set; }
        public decimal DescuentoGrupo { get; set; }
        public decimal DescuentoPromocional { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string Desglose { get; set; } = string.Empty;
    }
}
