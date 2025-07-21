using System;
using ProductsMicroService.BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.DTO;

public record ProductUpdateRequest
(
    Guid ProductId,
    string ProductName,
    double? UnitPrice,
    int? QuantityInStock,
    CategoryOptions Category
)
{
    public ProductUpdateRequest() :this(default,default,default,default,default)
    {
    }
};