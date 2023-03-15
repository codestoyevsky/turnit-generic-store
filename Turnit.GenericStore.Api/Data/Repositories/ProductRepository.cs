using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISession _session;

        public ProductRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _session.QueryOver<Product>().ListAsync();
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _session.GetAsync<Product>(id);
        }

        public async Task Save(string name, string description)
        {
            await _session.SaveAsync(new Product { Name = name, Description = description });
            await _session.FlushAsync();
        }

        public async Task<IEnumerable<Product>> GetAllByCategoryId(Guid categoryId)
        {
            return await _session.QueryOver<ProductCategory>()
                        .Where(x => x.Category.Id == categoryId)
                        .Select(x => x.Product)
                        .ListAsync<Product>();
        }

        public async Task<IEnumerable<Product>> GetAllUncategorized()
        {
            return await _session.QueryOver<ProductCategory>()
           .Where(x => x.Category.Id == null)
           .Select(x => x.Product)
           .ListAsync<Product>();
        }
    }
}