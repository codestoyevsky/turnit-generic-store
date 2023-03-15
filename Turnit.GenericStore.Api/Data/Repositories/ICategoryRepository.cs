using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetById(Guid id);

        Task Save(string name);

        Task<IEnumerable<Category>> GetAll();
    }
}
