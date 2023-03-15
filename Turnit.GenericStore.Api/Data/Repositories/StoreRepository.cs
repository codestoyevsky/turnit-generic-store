using System.Threading.Tasks;
using Turnit.GenericStore.Api.Entities;
using NHibernate;
using System;
using System.Collections.Generic;
using NHibernate.Linq;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ISession _session;

        public StoreRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Store> GetById(Guid id)
        {
            return await _session.GetAsync<Store>(id);
        }

        public async Task Save(string name)
        {
            await _session.SaveAsync(new Store { Name = name });
            await _session.FlushAsync();
        }

        public async Task<IEnumerable<Store>> GetAll()
        {
            return await _session.Query<Store>().ToListAsync();
        }
    }
}
