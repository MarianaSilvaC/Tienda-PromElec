using System.ComponentModel.DataAnnotations;

namespace TiendaPromElec.DTOs.Orders;

public sealed class UpdateOrderRequest
{
    [Required]
    [RegularExpression("^(Pendiente|Enviado|Entregado|Cancelado)$")]
    public string Status { get; init; } = string.Empty;
}
