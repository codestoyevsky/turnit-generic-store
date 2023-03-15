using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategory>> GetAll();
        Task AddProduct(Guid productId, Guid categoryId);
        Task RemoveProduct(Guid productId, Guid categoryId);
        Task<IEnumerable<ProductCategory>> GetAllByCategoryId(Guid categoryId);
        Task<IEnumerable<ProductCategory>> GetAll(IEnumerable<Guid> productIds);
    }
}
