using System.Threading.Tasks;
using Turnit.GenericStore.Api.Entities;
using NHibernate;
using System;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;

namespace Turnit.GenericStore.Api.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISession _session;

        public CategoryRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Category> GetById(Guid id)
        {
            return await _session.GetAsync<Category>(id);
        }

        public async Task Save(string name)
        {
            await _session.SaveAsync(new Category { Name = name });
            await _session.FlushAsync();
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _session.Query<Category>().ToListAsync();
        }
    }
}
