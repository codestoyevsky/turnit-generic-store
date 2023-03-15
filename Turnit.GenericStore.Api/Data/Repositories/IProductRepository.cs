using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetById(Guid id);
        Task Save(string name, string description);
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetAllByCategoryId(Guid categoryId);
        Task<IEnumerable<Product>> GetAllUncategorized();
    }
}
