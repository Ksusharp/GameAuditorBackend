using Microsoft.EntityFrameworkCore;
using Core.CommonModels.Enums;

namespace Core.Db.Ef
{
    public class EntityRepository<T> : IEntityRepository<T> where T : EntityBase
    {
        private readonly ApplicationContext context;
        private readonly DbSet<T> dbSet;
        public EntityRepository(ApplicationContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public T Get(Guid id)
        {
            return dbSet.Find(id);
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void CreateRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }
        public void Delete(Guid id)
        {
            T entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }
        public T GetName(string name)
        {
            return dbSet.Find(name);
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
