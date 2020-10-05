namespace XmlRepository.Base.Data
{
    using System.Collections.Generic;

    public interface IDataSource<T>
    {
        IEnumerable<T> Read();
        bool Write();
    }
}
