using Core.CommonModels.Enums;

namespace Core.Db
{
    public interface IEntityRepository<T> where T : EntityBase
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        void Create(T entity);
        void CreateRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(Guid id);
        T GetName(string tag);
        void Save();
    }
}
