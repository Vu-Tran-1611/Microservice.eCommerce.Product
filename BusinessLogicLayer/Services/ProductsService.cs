using System;
using System.Linq.Expressions;
using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using FluentValidation;
using FluentValidation.Results;
using ProductsMicroService.DataAccessLayer.RepositoryContracts;
namespace BusinessLogicLayer.Services;

public class ProductsService : IProductsService
{
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;

    private readonly IMapper _mapper;
    
    private readonly IProductsRepository _productsRepository;   

    public ProductsService(
        IValidator<ProductAddRequest> productAddRequestValidator,
        IValidator<ProductUpdateRequest> productUpdateRequestValidator,
        IMapper mapper,
        IProductsRepository productsRepository)
    {
        _productAddRequestValidator = productAddRequestValidator;
        _productUpdateRequestValidator = productUpdateRequestValidator;
        _mapper = mapper;
        _productsRepository = productsRepository;
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        // Implementation for getting all products
        IEnumerable<Product?> products = await _productsRepository.GetProducts();
        if (products == null || !products.Any())
        {
            return null;
        }
        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
        return productResponses.ToList();
    }


    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        // Implementation for getting products by condition
        IEnumerable<Product?> products = await _productsRepository.GetProductsByCondition(conditionExpression);
        if (products == null || !products.Any())
        {
            return null;
        }
        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
        return productResponses.ToList();
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        // Implementation for getting a product by condition
        Product? product = await _productsRepository.GetProductByCondition(conditionExpression);
        if (product == null)
        {
            return null;
        }
        ProductResponse? productResponse = _mapper.Map<ProductResponse?>(product);
        return productResponse;
    }

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }
        // Validate the product using Fluent Validation
        ValidationResult validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);
        if (!validationResult.IsValid)
        {
            string errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errorMessage);
        }

        // Map the DTO to the entity
        Product product = _mapper.Map<Product>(productAddRequest);

        // Add the product to the repository
        Product? addedProduct = await _productsRepository.AddProduct(product);
        if (addedProduct == null)
        {
            throw new InvalidOperationException("Failed to add the product.");
        }
        // Map the entity back to the DTO
        ProductResponse? addedProductResponse = _mapper.Map<ProductResponse?>(addedProduct);
        return addedProductResponse;
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        Product? existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productUpdateRequest.ProductId);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found.");
        }
        // Implementation for updating a product
        if (productUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(productUpdateRequest));
        }

        // Validate the product using Fluent Validation
        ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
        if (!validationResult.IsValid)
        {
            string errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errorMessage);
        }

        // Map the DTO to the entity
        Product product = _mapper.Map<Product>(productUpdateRequest);

        // Update the product in the repository
        Product? updatedProduct = await _productsRepository.UpdateProduct(product);
        if (updatedProduct == null)
        {
            throw new InvalidOperationException("Failed to update the product.");
        }

        // Map the entity back to the DTO
        ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse?>(updatedProduct);
        return updatedProductResponse;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        // Implementation for deleting a product
        Product? existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productId);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found.");
        }
        //Attempt to delete the product
        bool isDeleted = await _productsRepository.DeleteProduct(productId);
        return isDeleted;
    }
}
