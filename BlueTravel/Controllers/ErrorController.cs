using BlueTravel.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlueTravel.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorController(
            ILogger<ErrorController> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Path = exceptionFeature?.Path
            };

            if (exceptionFeature?.Error != null)
            {
                // Log del error
                _logger.LogError(
                    exceptionFeature.Error,
                    "Error no manejado en la ruta: {Path}. RequestId: {RequestId}",
                    exceptionFeature.Path,
                    errorViewModel.RequestId
                );

                // Solo mostrar detalles en Development
                if (_env.IsDevelopment())
                {
                    errorViewModel.ErrorMessage = exceptionFeature.Error.Message;
                    errorViewModel.StackTrace = exceptionFeature.Error.StackTrace;
                }
            }

            return View("Error", errorViewModel);
        }

        [Route("Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HandleStatusCode(int statusCode)
        {
            var statusCodeFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode,
                Path = statusCodeFeature?.OriginalPath
            };

            // Log del error de código de estado
            _logger.LogWarning(
                "Error de código de estado {StatusCode} en la ruta: {Path}. RequestId: {RequestId}",
                statusCode,
                statusCodeFeature?.OriginalPath,
                errorViewModel.RequestId
            );

            // Mensajes personalizados según el código de estado
            errorViewModel.ErrorMessage = statusCode switch
            {
                404 => "La página que buscas no existe.",
                403 => "No tienes permisos para acceder a este recurso.",
                500 => "Error interno del servidor.",
                _ => "Ha ocurrido un error inesperado."
            };

            return View("Error", errorViewModel);
        }
    }
}
