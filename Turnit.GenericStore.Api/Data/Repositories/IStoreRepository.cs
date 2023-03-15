using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public interface IStoreRepository
    {
        Task<Store> GetById(Guid id);

        Task<IEnumerable<Store>> GetAll();

        Task Save(string name);
    }
}
