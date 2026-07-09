namespace MisFinanzas.API.Middleware
{
    /// <summary>
    /// Formato estándar de respuesta ante errores (RNF-03 del SRS).
    /// </summary>
    public class ErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> Details { get; set; } = new();
    }
}