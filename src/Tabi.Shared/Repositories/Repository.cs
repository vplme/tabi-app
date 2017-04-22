using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using Tabi.Model;

namespace Tabi
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private SQLiteAsyncConnection db;

        public Repository(SQLiteAsyncConnection connection)
        {
            db = connection;
        }
        public AsyncTableQuery<T> AsQueryable()
        {
            return db.Table<T>();
        }

        public async Task<List<T>> Get()
        {
            return await db.Table<T>().ToListAsync();
        }

        public async Task<T> Get(int id)
        {
            return await db.FindAsync<T>(id);
        }

        public async Task<int> Insert(T entity) =>
             await db.InsertAsync(entity);


        public async Task<int> Update(T entity)
        {
            return await db.UpdateAsync(entity);
        }

        public async Task<int> Delete(T entity)
        {
            return await db.DeleteAsync(entity);
        }

        public async Task<List<T>> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            var query = db.Table<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy<TValue>(orderBy);

            return await query.ToListAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate) =>
          await db.FindAsync<T>(predicate);

    }
}
