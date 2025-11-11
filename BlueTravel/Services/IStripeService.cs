using Stripe;

namespace BlueTravel.Services
{
    /// <summary>
    /// Servicio para procesar pagos con Stripe
    /// Modo TEST - 100% GRATIS para desarrollo y académico
    /// </summary>
    public interface IStripeService
    {
        Task<PaymentIntentCreateResult> CrearIntencionPago(decimal monto, string moneda = "usd");
        Task<bool> ConfirmarPago(string paymentIntentId);
        Task<string> CrearReembolso(string chargeId, decimal monto);
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripeService> _logger;

        public StripeService(IConfiguration configuration, ILogger<StripeService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Configurar API Key de Stripe
            var secretKey = _configuration["Stripe:SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey) || secretKey.Contains("REEMPLAZA"))
            {
                _logger.LogWarning("?? Stripe no configurado. Usando modo simulación.");
            }
            else
            {
                StripeConfiguration.ApiKey = secretKey;
                _logger.LogInformation("? Stripe configurado en modo TEST");
            }
        }

        /// <summary>
        /// Crear intención de pago en Stripe
        /// </summary>
        public async Task<PaymentIntentCreateResult> CrearIntencionPago(decimal monto, string moneda = "usd")
        {
            try
            {
                // Verificar si Stripe está configurado
                if (string.IsNullOrEmpty(StripeConfiguration.ApiKey) || 
                    StripeConfiguration.ApiKey.Contains("REEMPLAZA"))
                {
                    _logger.LogInformation("Modo simulación - Stripe no configurado");
                    return new PaymentIntentCreateResult
                    {
                        Success = true,
                        PaymentIntentId = $"pi_sim_{Guid.NewGuid().ToString()[..8]}",
                        ClientSecret = $"sim_secret_{Guid.NewGuid().ToString()[..16]}",
                        Simulado = true
                    };
                }

                // Stripe real - Crear Payment Intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(monto * 100), // Stripe usa centavos
                    Currency = moneda,
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "proyecto", "BlueTravel" },
                        { "modo", "TEST" }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                _logger.LogInformation("? Payment Intent creado: {Id}", paymentIntent.Id);

                return new PaymentIntentCreateResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Simulado = false
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "? Error de Stripe: {Message}", ex.Message);
                return new PaymentIntentCreateResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Confirmar pago
        /// </summary>
        public async Task<bool> ConfirmarPago(string paymentIntentId)
        {
            try
            {
                // Modo simulación
                if (paymentIntentId.StartsWith("pi_sim_"))
                {
                    _logger.LogInformation("? Pago simulado confirmado: {Id}", paymentIntentId);
                    await Task.Delay(500); // Simular latencia
                    return true;
                }

                // Stripe real
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                bool exitoso = paymentIntent.Status == "succeeded";
                
                _logger.LogInformation(exitoso 
                    ? "? Pago confirmado: {Id}" 
                    : "?? Pago no completado: {Id} - Estado: {Status}", 
                    paymentIntentId, paymentIntent.Status);

                return exitoso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al confirmar pago: {Id}", paymentIntentId);
                return false;
            }
        }

        /// <summary>
        /// Crear reembolso
        /// </summary>
        public async Task<string> CrearReembolso(string chargeId, decimal monto)
        {
            try
            {
                // Modo simulación
                if (chargeId.StartsWith("pi_sim_"))
                {
                    var refundId = $"re_sim_{Guid.NewGuid().ToString()[..8]}";
                    _logger.LogInformation("? Reembolso simulado: {Id}", refundId);
                    await Task.Delay(300);
                    return refundId;
                }

                // Stripe real
                var options = new RefundCreateOptions
                {
                    Charge = chargeId,
                    Amount = (long)(monto * 100)
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                _logger.LogInformation("? Reembolso creado: {Id}", refund.Id);
                return refund.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al crear reembolso");
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Resultado de creación de Payment Intent
    /// </summary>
    public class PaymentIntentCreateResult
    {
        public bool Success { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Simulado { get; set; }
    }
}
