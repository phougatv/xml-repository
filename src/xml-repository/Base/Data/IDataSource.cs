using System.Collections.Generic;

namespace XmlRepository.Base.Data
{
    public interface IDataSource<TEntity>
    {
        bool Write(IEnumerable<TEntity> entities);
        SourceEntity<TEntity> Read();
    }
}
