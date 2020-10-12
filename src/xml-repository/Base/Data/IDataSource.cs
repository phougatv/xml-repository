using System.Collections.Generic;

namespace XmlRepository.Base.Data
{
    public interface IDataSource<TEntity>
    {
        bool Commit(IEnumerable<TEntity> entities);
        SourceEntity<TEntity> Read();
    }
}
