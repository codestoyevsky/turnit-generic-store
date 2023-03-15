using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public class ProductAvailabilityRepository : IProductAvailabilityRepository
    {
        private readonly ISession _session;

        public ProductAvailabilityRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<ProductAvailability>> GetAll(List<Guid> productIds)
        {
            return await _session.QueryOver<ProductAvailability>()
                .Fetch(x => x.Product).Eager
                .Fetch(x => x.Store).Eager
                .Where(x => x.Product.Id.IsIn(productIds.ToList()))
                .ListAsync();
        }

        public async Task<IEnumerable<ProductAvailability>> GetAll(Guid productId)
        {
            return await _session.QueryOver<ProductAvailability>()
                .Where(x => x.Product.Id == productId)
                .ListAsync();
        }

        public async Task<ProductAvailability> Get(Guid productId, Guid storeId)
        {
            return await _session.QueryOver<ProductAvailability>()
                .Where(x => x.Product.Id == productId && x.Store.Id == storeId).SingleOrDefaultAsync();
        }

        public async Task Book(Guid productId, Dictionary<Guid, int> storeAndQuantityPair)
        {
            var productAvailibilities = await _session.QueryOver<ProductAvailability>()
                .Fetch(x => x.Product).Eager
                .Fetch(x => x.Store).Eager
                .Where(x => x.Product.Id == productId && x.Store.Id.IsIn(storeAndQuantityPair.Keys))
                .ListAsync();

            foreach (var productAvailibilty in productAvailibilities)
            {
                var quantity = 0;
                storeAndQuantityPair.TryGetValue(productAvailibilty.Store.Id, out quantity);
                productAvailibilty.Availability -= quantity;
                if (productAvailibilty.Availability < 0) continue;
                await _session.UpdateAsync(productAvailibilty);

            }
            await _session.FlushAsync();
        }

        public async Task Restock(Guid storeId, Dictionary<Guid, int> productAndQuantityPairs)
        {
            var productAvailibilties = new List<ProductAvailability>();
            var store = await _session.GetAsync<Store>(storeId);
            var existingProductAvailibilities = await _session.QueryOver<ProductAvailability>()
            .Fetch(x => x.Product).Eager
            .Fetch(x => x.Store).Eager
            .Where(x => x.Store.Id == storeId && x.Product.Id.IsIn(productAndQuantityPairs.Keys.ToList()))
            .ListAsync<ProductAvailability>();


            foreach (var productAvailibilty in existingProductAvailibilities)
            {
                var quantity = 0;
                productAndQuantityPairs.TryGetValue(productAvailibilty.Product.Id, out quantity);
                productAvailibilty.Availability += quantity;
                await _session.SaveOrUpdateAsync(productAvailibilty);
            }

            foreach (var productAndQuantity in productAndQuantityPairs)
            {

                if (existingProductAvailibilities.Any(x => productAndQuantity.Key == x.Product.Id)) continue;
                var product = await _session.QueryOver<Product>().Where(x => x.Id == productAndQuantity.Key).SingleOrDefaultAsync();

                var newProductAvailability = new ProductAvailability
                {
                    Product = product,
                    Store = store,
                    Availability = productAndQuantity.Value
                };
                await _session.SaveOrUpdateAsync(newProductAvailability);

            }

            await _session.FlushAsync();
        }
    }
}