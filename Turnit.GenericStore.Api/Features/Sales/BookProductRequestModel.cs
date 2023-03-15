using System;

namespace Turnit.GenericStore.Api.Features.Sales;

public class BookProductRequestModel
{
    public Guid StoreId { get; set; }

    public int Quantity { get; set; }
}