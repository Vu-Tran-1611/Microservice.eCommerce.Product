using System;
using System.Linq.Expressions;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using ProductsMicroService.DataAccessLayer.Context;
using ProductsMicroService.DataAccessLayer.RepositoryContracts;

namespace ProductsMicroService.DataAccessLayer.Repository;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _context;
    public ProductsRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product?>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<Product?> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        Product? existingProduct = _context.Products.FirstOrDefault(t => t.ProductID == product.ProductID);
        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.Category = product.Category;
        existingProduct.QuantityInStock = product.QuantityInStock;
        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        Product? existingProduct = await _context.Products.FirstOrDefaultAsync(t => t.ProductID == productId);
        if (existingProduct == null)
        {
            return false;
        }
        _context.Products.Remove(existingProduct);
        int affectedRowsCount = await _context.SaveChangesAsync();

        return affectedRowsCount > 0;
    }
    
}
