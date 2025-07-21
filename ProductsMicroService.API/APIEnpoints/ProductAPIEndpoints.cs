using System;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;
using MySqlX.XDevAPI.Common;

namespace ProductsMicroService.API.APIEnpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // Define your product-related endpoints here
        //GET /api/products
        app.MapGet("/api/products", async (IProductsService productsService) =>
        {
            List<ProductResponse?> products= await productsService.GetProducts();
            return Results.Ok(products);
        });

        //GET /api/products/{ProductID}
        app.MapGet("/api/products/{ProductID:guid}", async (IProductsService productsService, Guid ProductID) =>
        {
            ProductResponse? productResponse = await productsService.GetProductByCondition(temp => temp.ProductID == ProductID);
            if (productResponse == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(productResponse);
        });

        //GET /api/products/search/{searchString}
        app.MapGet("/api/products/search/{searchString}", async (IProductsService productsService, string searchString) =>
        {
            List<ProductResponse?> productsByName = await productsService.GetProductsByCondition( p => p.ProductName != null && p.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ?? new List<ProductResponse?>();
            List<ProductResponse?> productsByCategory = await productsService.GetProductsByCondition( p => p.Category != null && p.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ?? new List<ProductResponse?>();
            var products = productsByName.Union(productsByCategory).Where(p => p != null).Distinct().ToList();
            return Results.Ok(products);
        });
      

        //POST /api/products
        app.MapPost("/api/products", async (IProductsService productsService, IValidator<ProductAddRequest> validator, ProductAddRequest productAddRequest) =>
        {
            if (productAddRequest == null)
            {
                return Results.Problem("Product data is required.");
            }
            
            ValidationResult validationResult = await validator.ValidateAsync(productAddRequest);

            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

            ProductResponse? addedProductResponse = await productsService.AddProduct(productAddRequest);
            if (addedProductResponse == null)
            {
                return Results.BadRequest("Failed to add product.");
            }
            return Results.Created($"/api/products/{addedProductResponse.ProductId}", addedProductResponse);
        });

        //PUT /api/products/{ProductID}
        app.MapPut("/api/products", async (IProductsService productsService, IValidator<ProductUpdateRequest> validator, ProductUpdateRequest productUpdateRequest) =>
        {   
            if(productUpdateRequest == null)
            {
                return Results.BadRequest("Product data is required.");
            }

            ValidationResult validationResult = await validator.ValidateAsync(productUpdateRequest);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

            ProductResponse? updatedProductResponse = await productsService.UpdateProduct(productUpdateRequest);
            if (updatedProductResponse == null)
            {
                return Results.BadRequest("Failed to update product.");
            }
            return Results.Ok(updatedProductResponse);
        });


        //DELETE /api/products/{ProductID}
        app.MapDelete("/api/products/{ProductID:guid}", async (IProductsService productsService, Guid ProductID) =>
        {
            bool isDeleted = await productsService.DeleteProduct(ProductID);
            if (!isDeleted)
            {
                return Results.Problem("Failed to delete product.");
            }
            return Results.Ok(true);
        });

        return app;
    } 
}
