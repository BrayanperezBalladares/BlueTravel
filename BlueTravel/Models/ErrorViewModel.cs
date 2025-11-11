namespace BlueTravel.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // ? NUEVO: Agregar soporte para código de estado HTTP
        public int? StatusCode { get; set; }

        public string? ErrorMessage { get; set; }

        public string? StackTrace { get; set; }

        public string? Path { get; set; }
    }
}
