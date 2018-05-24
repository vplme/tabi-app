using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Test
{
    public class MockUploadEntryRepository : IUploadEntryRepository
    {
        public List<UploadEntry> UploadEntries { get; private set; } = new List<UploadEntry>();

        public void Add(UploadEntry entity)
        {
            UploadEntries.Add(entity);
        }

        public void AddRange(IEnumerable<UploadEntry> entities)
        {
            UploadEntries.AddRange(entities);
        }

        public int Count()
        {
            return UploadEntries.Count;
        }

        public IEnumerable<UploadEntry> Find(Expression<Func<UploadEntry, bool>> predicate)
        {
            throw new NotImplementedException();

        }

        public UploadEntry Get(object id)
        {
            return UploadEntries.Where(x => x.Id == (int)id).FirstOrDefault();
        }

        public IEnumerable<UploadEntry> GetAll()
        {
            throw new NotImplementedException();
        }

        public UploadEntry GetLastUploadEntry(UploadType type)
        {
            return UploadEntries.Where(x => x.Type == type).LastOrDefault();
        }

        public void Remove(UploadEntry entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<UploadEntry> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(UploadEntry entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateAll(IEnumerable<UploadEntry> entities)
        {
            throw new NotImplementedException();
        }
    }
}
