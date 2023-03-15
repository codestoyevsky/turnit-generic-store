using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public interface IProductAvailabilityRepository
    {
        Task<IEnumerable<ProductAvailability>> GetAll(Guid productId);

        Task<IEnumerable<ProductAvailability>> GetAll(List<Guid> productIds);

        Task<ProductAvailability> Get(Guid productId, Guid storeId);

        Task Book(Guid productId, Dictionary<Guid, int> storeAndQuantityPair);

        Task Restock(Guid storeId, Dictionary<Guid, int> productAndQuantityPair);
    }
}
