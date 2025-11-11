using BlueTravel.Data;
using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Services
{
    public class PrecioService : IPrecioService
    {
        private const decimal IVA_COSTA_RICA = 0.13m; // 13%
        private readonly ApplicationDbContext? _context;
        private readonly ILogger<PrecioService>? _logger;

        // Constructor principal con dependencias
        public PrecioService(ApplicationDbContext context, ILogger<PrecioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ? Constructor sin parámetros para tests
        public PrecioService()
        {
            _context = null;
            _logger = null;
        }

        public async Task<ResultadoCalculo> CalcularPrecioHospedaje(
            Hospedaje hospedaje,
            DateTime fechaInicio,
            DateTime fechaFin,
            int cantidadAdultos,
            int cantidadNinos,
            int cantidadSeniors)
        {
            // ? VALIDACIONES
            if (hospedaje == null)
                throw new ArgumentNullException(nameof(hospedaje));
            
            if (fechaFin <= fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
            
            var totalPersonas = cantidadAdultos + cantidadNinos + cantidadSeniors;
            if (totalPersonas <= 0)
                throw new ArgumentException("Debe haber al menos una persona en la reserva");
            
            var resultado = new ResultadoCalculo();
            var dias = Math.Max(1, (fechaFin - fechaInicio).Days);

            _logger?.LogInformation("Calculando precio hospedaje: {Hospedaje}, {Dias} días, {Personas} personas",
                hospedaje.Nombre, dias, totalPersonas);

            // Precio base (precio por noche × días)
            resultado.PrecioBase = hospedaje.PrecioPorNoche * dias;

            // Cargo por personas extra
            var personasExtra = Math.Max(0, totalPersonas - hospedaje.PersonasIncluidasEnPrecio);
            resultado.CargoPersonasExtra = personasExtra * hospedaje.CargoPorPersonaExtra * dias;

            // Descuentos por estancias largas
            if (dias >= 14)
            {
                resultado.DescuentoPromocional = resultado.PrecioBase * 0.15m; // 15% desc. por 2 semanas
                _logger?.LogInformation("Descuento de 15% aplicado por estancia de {Dias} días", dias);
            }
            else if (dias >= 7)
            {
                resultado.DescuentoPromocional = resultado.PrecioBase * 0.10m; // 10% desc. por semana
                _logger?.LogInformation("Descuento de 10% aplicado por estancia de {Dias} días", dias);
            }

            // Cálculos finales
            resultado.Subtotal = resultado.PrecioBase + resultado.CargoPersonasExtra - resultado.DescuentoPromocional;
            resultado.Impuestos = resultado.Subtotal * IVA_COSTA_RICA;
            resultado.Total = resultado.Subtotal + resultado.Impuestos;

            resultado.Desglose = $"Base: {resultado.PrecioBase:C} ({hospedaje.PrecioPorNoche:C}/noche × {dias} días) | " +
                                 $"Extra: {resultado.CargoPersonasExtra:C} ({personasExtra} personas × {hospedaje.CargoPorPersonaExtra:C}/noche × {dias} días) | " +
                                 $"Desc: -{resultado.DescuentoPromocional:C} | " +
                                 $"Subtotal: {resultado.Subtotal:C} | " +
                                 $"IVA (13%): {resultado.Impuestos:C} | " +
                                 $"TOTAL: {resultado.Total:C}";

            _logger?.LogInformation("Precio calculado: {Total:C}", resultado.Total);

            return resultado;
        }

        public async Task<ResultadoCalculo> CalcularPrecioTour(
            Tour tour,
            int cantidadAdultos,
            int cantidadNinos,
            int cantidadSeniors)
        {
            // ? VALIDACIONES
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));
            
            var totalPersonas = cantidadAdultos + cantidadNinos + cantidadSeniors;
            if (totalPersonas <= 0)
                throw new ArgumentException("Debe haber al menos una persona en la reserva");
            
            var resultado = new ResultadoCalculo();

            _logger?.LogInformation("Calculando precio tour: {Tour}, {Personas} personas",
                tour.Nombre, totalPersonas);

            // Precio diferenciado por edad
            var precioAdultos = cantidadAdultos * tour.Precio;
            var precioNinos = cantidadNinos * (tour.PrecioNino ?? tour.Precio);
            var precioSeniors = cantidadSeniors * (tour.PrecioSenior ?? tour.Precio);

            resultado.PrecioBase = precioAdultos + precioNinos + precioSeniors;

            // Descuento de grupo (si aplica)
            if (totalPersonas >= 10 && tour.DescuentoGrupo > 0)
            {
                resultado.DescuentoGrupo = resultado.PrecioBase * (tour.DescuentoGrupo / 100m);
                _logger?.LogInformation("Descuento de grupo de {Porcentaje}% aplicado para {Personas} personas",
                    tour.DescuentoGrupo, totalPersonas);
            }

            // Cálculos finales
            resultado.Subtotal = resultado.PrecioBase - resultado.DescuentoGrupo;
            resultado.Impuestos = resultado.Subtotal * IVA_COSTA_RICA;
            resultado.Total = resultado.Subtotal + resultado.Impuestos;

            resultado.Desglose = $"Adultos: {precioAdultos:C} ({cantidadAdultos} × {tour.Precio:C}) | " +
                                 $"Niños: {precioNinos:C} ({cantidadNinos} × {tour.PrecioNino ?? tour.Precio:C}) | " +
                                 $"Seniors: {precioSeniors:C} ({cantidadSeniors} × {tour.PrecioSenior ?? tour.Precio:C}) | " +
                                 $"Desc. Grupo ({tour.DescuentoGrupo}%): -{resultado.DescuentoGrupo:C} | " +
                                 $"Subtotal: {resultado.Subtotal:C} | " +
                                 $"IVA (13%): {resultado.Impuestos:C} | " +
                                 $"TOTAL: {resultado.Total:C}";

            _logger?.LogInformation("Precio calculado: {Total:C}", resultado.Total);

            return resultado;
        }

        public async Task<decimal> AplicarDescuentosPromocionales(
            string tipoReserva,
            int itemId,
            decimal precioBase)
        {
            // TODO: Aquí podrías integrar con tabla de promociones activas
            // Por ahora, retorna 0
            return 0m;
        }

        public decimal CalcularImpuestos(decimal precioBase)
        {
            return precioBase * IVA_COSTA_RICA;
        }
    }
}
