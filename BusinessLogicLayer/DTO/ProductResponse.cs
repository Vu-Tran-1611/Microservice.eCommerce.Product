using System;
using ProductsMicroService.BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.DTO;

public record ProductResponse
(
    Guid ProductId,
    string ProductName,
    double? UnitPrice,
    int? QuantityInStock,
    CategoryOptions Category
)
{
    public ProductResponse() : this(default, default, default, default, default)
    {
    }
};