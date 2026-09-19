using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
using TiendaPromElec.DTOs.Products;
using TiendaPromElec.Exceptions;
using TiendaPromElec.Services.Interfaces;

namespace TiendaPromElec.Services;

public sealed class ProductService(AppDbContext context) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Product {id} was not found.");

        return ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Brand = request.Brand.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl.Trim(),
            CategoryId = request.CategoryId
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task UpdateAsync(
        long id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Product {id} was not found.");

        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Brand = request.Brand.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl.Trim();
        product.CategoryId = request.CategoryId;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new ResourceNotFoundException($"Product {id} was not found.");

        var isUsedByOrder = await context.OrderDetails
            .AnyAsync(detail => detail.ProductId == id, cancellationToken);

        if (isUsedByOrder)
        {
            throw new ConflictException("Products referenced by an order cannot be deleted.");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryExistsAsync(long categoryId, CancellationToken cancellationToken)
    {
        if (!await context.Categories.AnyAsync(category => category.Id == categoryId, cancellationToken))
        {
            throw new ResourceNotFoundException($"Category {categoryId} was not found.");
        }
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Brand,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.CategoryId);
    }
}
