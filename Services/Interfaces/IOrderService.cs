using TiendaPromElec.DTOs.Orders;

namespace TiendaPromElec.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(long id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
