using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlueTravel.Security
{
    /// <summary>
    /// Filtro personalizado para prevenir abuso de creación de reservas
    /// Limita a 5 reservas por usuario por hora
    /// </summary>
    public class RateLimitReservasAttribute : ActionFilterAttribute
    {
        private static readonly Dictionary<string, List<DateTime>> _userRequests = new();
        private const int MAX_REQUESTS_PER_HOUR = 5;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                base.OnActionExecuting(context);
                return;
            }

            lock (_userRequests)
            {
                if (!_userRequests.ContainsKey(userId))
                {
                    _userRequests[userId] = new List<DateTime>();
                }

                // Limpiar requests antiguos (más de 1 hora)
                _userRequests[userId].RemoveAll(dt => dt < DateTime.Now.AddHours(-1));

                // Verificar límite
                if (_userRequests[userId].Count >= MAX_REQUESTS_PER_HOUR)
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = 429, // Too Many Requests
                        Content = "Has excedido el límite de reservas por hora. Intenta más tarde."
                    };
                    return;
                }

                // Registrar request
                _userRequests[userId].Add(DateTime.Now);
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Filtro de auditoría para acciones sensibles
    /// </summary>
    public class AuditLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<AuditLogAttribute>>();

            var userId = context.HttpContext.User?.Identity?.Name ?? "Anonymous";
            var action = context.ActionDescriptor.DisplayName;
            var result = context.Result;

            logger.LogInformation(
                "AUDIT: User={User}, Action={Action}, Success={Success}",
                userId, action, result is not BadRequestResult);

            base.OnActionExecuted(context);
        }
    }

    /// <summary>
    /// Extensiones de seguridad
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Enmascara números de tarjeta
        /// </summary>
        public static string EnmascararTarjeta(this string numeroTarjeta)
        {
            if (string.IsNullOrEmpty(numeroTarjeta) || numeroTarjeta.Length < 4)
                return "****";

            var ultimos4 = numeroTarjeta[^4..];
            return $"**** **** **** {ultimos4}";
        }

        /// <summary>
        /// Valida formato de email básico
        /// </summary>
        public static bool EsEmailValido(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sanitiza entrada de usuario (prevención XSS básica)
        /// </summary>
        public static string SanitizarEntrada(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Net.WebUtility.HtmlEncode(input);
        }
    }
}
