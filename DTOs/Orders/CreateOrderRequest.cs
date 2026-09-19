using System.ComponentModel.DataAnnotations;

namespace TiendaPromElec.DTOs.Orders;

public sealed class CreateOrderRequest
{
    [Range(typeof(long), "1", "9223372036854775807")]
    public long CustomerId { get; init; }

    [Required]
    [MinLength(1)]
    public ICollection<CreateOrderItemRequest> Items { get; init; } = [];
}
