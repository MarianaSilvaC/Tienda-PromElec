using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
using TiendaPromElec.DTOs.Orders;
using TiendaPromElec.Exceptions;
using TiendaPromElec.Services.Interfaces;

namespace TiendaPromElec.Services;

public sealed class OrderService(AppDbContext context) : IOrderService
{
    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderBy(order => order.Id)
            .ToListAsync(cancellationToken);

        return orders.Select(ToResponse).ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Order {id} was not found.");

        return ToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await context.Customers.AnyAsync(customer => customer.Id == request.CustomerId, cancellationToken))
        {
            throw new ResourceNotFoundException($"Customer {request.CustomerId} was not found.");
        }

        var requestedItems = request.Items.ToList();
        if (requestedItems.Select(item => item.ProductId).Distinct().Count() != requestedItems.Count)
        {
            throw new ConflictException("An order cannot contain the same product more than once.");
        }

        var productIds = requestedItems.Select(item => item.ProductId).ToList();
        var products = await context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        var missingProductId = productIds.FirstOrDefault(productId => !products.ContainsKey(productId));
        if (missingProductId != 0)
        {
            throw new ResourceNotFoundException($"Product {missingProductId} was not found.");
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = "Pendiente"
        };

        foreach (var requestedItem in requestedItems)
        {
            var product = products[requestedItem.ProductId];
            if (product.Stock < requestedItem.Quantity)
            {
                throw new ConflictException($"Product {product.Id} does not have enough stock.");
            }

            product.Stock -= requestedItem.Quantity;
            order.Items.Add(new OrderDetail
            {
                ProductId = product.Id,
                Product = product,
                Quantity = requestedItem.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(item => item.UnitPrice * item.Quantity);

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }

    public async Task UpdateAsync(
        long id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Order {id} was not found.");

        order.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Order {id} was not found.");

        var productIds = order.Items.Select(item => item.ProductId).ToList();
        var products = await context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        foreach (var item in order.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                product.Stock += item.Quantity;
            }
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static OrderResponse ToResponse(Order order)
    {
        var items = order.Items
            .OrderBy(item => item.Id)
            .Select(item => new OrderItemResponse(
                item.Id,
                item.ProductId,
                item.Quantity,
                item.UnitPrice))
            .ToList();

        return new OrderResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.CustomerId,
            items);
    }
}
