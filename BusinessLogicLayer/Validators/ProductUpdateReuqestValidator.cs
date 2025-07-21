using System;
using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductUpdateRequestValidator:AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.QuantityInStock)
            .NotEmpty().WithMessage("Quantity in stock is required.").InclusiveBetween(0, int.MaxValue)
            .WithMessage("Quantity in stock must be a non-negative integer.");
        RuleFor(x => x.Category)
            .NotEmpty().IsInEnum().WithMessage("Category is required.");
    }
}
