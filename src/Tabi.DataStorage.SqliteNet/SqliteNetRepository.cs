using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SQLite;

namespace Tabi.DataStorage.SqliteNet
{
    public abstract class SqliteNetRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly SQLiteConnection connection;

        public SqliteNetRepository(SQLiteConnection conn)
        {
            this.connection = conn;
        }

        public void Add(TEntity entity)
        {
            try
            {
                connection.Insert(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not insert into database " + e);
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            connection.InsertAll(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return connection.Table<TEntity>().Where(predicate);
        }

        public TEntity Get(object id)
        {
            return connection.Find<TEntity>(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return connection.Table<TEntity>();
        }

        public void Remove(TEntity entity)
        {
            connection.Delete(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return connection.Table<TEntity>().Count();
        }

        public void Clear()
        {
            connection.DeleteAll<TEntity>();
        }
        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            connection.UpdateAll(entities);
        }
    }
}
