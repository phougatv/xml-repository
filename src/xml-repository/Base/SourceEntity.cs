namespace XmlRepository.Base
{
    using System.Collections.Generic;

    public class SourceEntity<TEntity>
    {
        public IEnumerable<TEntity> Entities { get; }
        public int EntitiesCount { get; }
        public IEnumerable<string> Errors { get; }
        public int ErrorsCount { get; }

        public SourceEntity(
            IEnumerable<TEntity> entities
            , IEnumerable<string> errors
            , int entitiesCount
            , int errorsCount)
        {
            Entities = entities;
            EntitiesCount = entitiesCount;
            Errors = errors;
            ErrorsCount = errorsCount;
        }
    }
}
