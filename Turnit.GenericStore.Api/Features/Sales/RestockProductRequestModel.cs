using System;

namespace Turnit.GenericStore.Api.Features.Sales;

public class RestockProductRequestModel
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}