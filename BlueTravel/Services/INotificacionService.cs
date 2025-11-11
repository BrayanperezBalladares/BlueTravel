using BlueTravel.Models;

namespace BlueTravel.Services
{
    /// <summary>
    /// Servicio para envío de notificaciones por email, SMS, etc.
    /// </summary>
    public interface INotificacionService
    {
        /// <summary>
        /// Envía confirmación de reserva al cliente
        /// </summary>
        Task EnviarConfirmacionReserva(Reserva reserva, string emailCliente);

        /// <summary>
        /// Envía notificación de cambio de estado
        /// </summary>
        Task EnviarCambioEstado(Reserva reserva, string estadoAnterior, string emailCliente);

        /// <summary>
        /// Envía recordatorio de reserva próxima
        /// </summary>
        Task EnviarRecordatorioReserva(Reserva reserva, string emailCliente);

        /// <summary>
        /// Envía solicitud de valoración post-servicio
        /// </summary>
        Task EnviarSolicitudValoracion(Reserva reserva, string emailCliente);

        /// <summary>
        /// Envía comprobante de pago
        /// </summary>
        Task EnviarComprobantePago(Pago pago, string emailCliente);

        /// <summary>
        /// Notifica al administrador de nueva reserva pendiente
        /// </summary>
        Task NotificarAdminNuevaReserva(Reserva reserva);
    }

    public class NotificacionService : INotificacionService
    {
        private readonly ILogger<NotificacionService> _logger;

        public NotificacionService(ILogger<NotificacionService> logger)
        {
            _logger = logger;
        }

        public async Task EnviarConfirmacionReserva(Reserva reserva, string emailCliente)
        {
            _logger.LogInformation("Enviando confirmación de reserva #{Id} a {Email}", 
                reserva.Id, emailCliente);

            // TODO: Integrar con servicio de email (SendGrid, SMTP, etc.)
            var asunto = $"Confirmación de Reserva #{reserva.Id} - BlueTravel";
            var cuerpo = $@"
                Estimado cliente,
                
                Tu reserva ha sido creada exitosamente.
                
                Detalles:
                - Tipo: {reserva.TipoReserva}
                - Item: {reserva.ItemNombre}
                - Fecha Inicio: {reserva.FechaInicio:dd/MM/yyyy}
                - Fecha Fin: {reserva.FechaFin:dd/MM/yyyy}
                - Personas: {reserva.DetallePersonas}
                - Total: {reserva.PrecioTotal:C}
                - Estado: {reserva.Estado}
                
                Gracias por confiar en BlueTravel.
            ";

            // Simular envío (en producción, usar SendGrid, etc.)
            await Task.Delay(100);
            _logger.LogInformation("? Confirmación enviada exitosamente");
        }

        public async Task EnviarCambioEstado(Reserva reserva, string estadoAnterior, string emailCliente)
        {
            _logger.LogInformation("Notificando cambio de estado de reserva #{Id}: {Anterior} -> {Nuevo}",
                reserva.Id, estadoAnterior, reserva.Estado);

            await Task.Delay(100);
            _logger.LogInformation("? Notificación de cambio de estado enviada");
        }

        public async Task EnviarRecordatorioReserva(Reserva reserva, string emailCliente)
        {
            _logger.LogInformation("Enviando recordatorio de reserva #{Id}", reserva.Id);
            await Task.Delay(100);
        }

        public async Task EnviarSolicitudValoracion(Reserva reserva, string emailCliente)
        {
            _logger.LogInformation("Solicitando valoración de reserva #{Id}", reserva.Id);
            await Task.Delay(100);
        }

        public async Task EnviarComprobantePago(Pago pago, string emailCliente)
        {
            _logger.LogInformation("Enviando comprobante de pago #{Id}", pago.Id);
            await Task.Delay(100);
        }

        public async Task NotificarAdminNuevaReserva(Reserva reserva)
        {
            _logger.LogInformation("Notificando admin de nueva reserva #{Id}", reserva.Id);
            await Task.Delay(100);
        }
    }
}
