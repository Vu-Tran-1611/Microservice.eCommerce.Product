using System;
using ProductsMicroService.BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.DTO;

public record ProductAddRequest
(
    string ProductName,
    double? UnitPrice,
    int? QuantityInStock,
    CategoryOptions Category
)
{
    public ProductAddRequest() :this(default,default,default,default)
    {
    }
};