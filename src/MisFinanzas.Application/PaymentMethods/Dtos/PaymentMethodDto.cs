namespace MisFinanzas.Application.PaymentMethods.Dtos
{
    /// <summary>Medio de pago del catálogo, para llenar listas en el cliente.</summary>
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}