using System;

namespace Turnit.GenericStore.Api.Features.Sales;

public class CategoryModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}

public class StoreModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}