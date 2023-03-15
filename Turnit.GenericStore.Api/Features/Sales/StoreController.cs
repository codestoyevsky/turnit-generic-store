using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Mapping;
using Turnit.GenericStore.Api.Data.Repositories;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("stores")]
public class StoresController : ApiControllerBase
{
    private readonly IStoreRepository _storeRepository;
    private readonly IProductAvailabilityRepository _productAvailabilityRepository;

    public StoresController(IStoreRepository storeRepository, IProductAvailabilityRepository productAvailabilityRepository)
    {
        _storeRepository = storeRepository;
        _productAvailabilityRepository = productAvailabilityRepository;
    }

    [HttpGet, Route("")]
    public async Task<StoreModel[]> AllStores()
    {
        var stores = await _storeRepository.GetAll();

        var result = stores
            .Select(x => new StoreModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToArray();

        return result;
    }

    [HttpPost, Route("{storeId:guid}/restock")]
    public async Task<IActionResult> RestockProduct(Guid storeId, List<RestockProductRequestModel> request)
    {
        _productAvailabilityRepository.Restock(storeId, request.ToDictionary(x => x.ProductId, x => x.Quantity));
        return Ok();
    }


    [HttpPost, Route("")]
    public async Task<IActionResult> Save(string name)
    {
        await _storeRepository.Save(name);
        return Ok();
    }
}