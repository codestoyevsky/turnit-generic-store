using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ISession _session;

        public ProductCategoryRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<ProductCategory>> GetAll()
        {
            return await _session.QueryOver<ProductCategory>()
                            .Fetch(x => x.Product).Eager
                            .Fetch(x => x.Category).Eager
                            .ListAsync<ProductCategory>();
        }

        public async Task<IEnumerable<ProductCategory>> GetAllByCategoryId(Guid categoryId)
        {
            return await _session.QueryOver<ProductCategory>()
                .Where(x => x.Category.Id == categoryId)
                .ListAsync();
        }

        public async Task RemoveProduct(Guid productId, Guid categoryId)
        {
            var productCategory = await _session.QueryOver<ProductCategory>()
                .Where(x => x.Product.Id == productId && x.Category.Id == categoryId)
                .SingleOrDefaultAsync<ProductCategory>();

            if (productCategory != null)
            {
                await _session.DeleteAsync(productCategory);
                await _session.FlushAsync();
            }
        }

        public async Task AddProduct(Guid productId, Guid categoryId)
        {
            var product = await _session.GetAsync<Product>(productId);
            var category = await _session.GetAsync<Category>(categoryId);

            if (product != null && category != null)
            {
                var productCategory = new ProductCategory
                {
                    Product = product,
                    Category = category
                };
                await _session.SaveAsync(productCategory);
                await _session.FlushAsync();
            }
        }

        public async Task<IEnumerable<ProductCategory>> GetAll(IEnumerable<Guid> productIds)
        {
            return await _session.QueryOver<ProductCategory>()
                         .Fetch(x => x.Product).Eager
                         .Fetch(x => x.Category).Eager
                         .Where(x => x.Product.Id.IsIn(productIds.ToList()))
                         .ListAsync();
        }
    }
}