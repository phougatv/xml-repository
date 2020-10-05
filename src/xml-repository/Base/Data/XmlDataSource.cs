namespace XmlRepository.Base.Data
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using XmlRepository.Configuration;
    using static XmlRepository.InternalExtension;

    public class XmlDataSource<T> : IDataSource<T>
        where T : XmlBaseEntity, new()
    {
        #region Private Variables
        private readonly DataSource _dataSource;
        private List<T> _items;
        #endregion

        #region Ctors
        /// <summary>
        /// Gets an intance of <see cref="XmlDataSource{T}"/>
        /// </summary>
        /// <param name="absoluteFilePath">Fully qualified XML file name including the file name with extension.</param>
        public XmlDataSource(IConfiguration configuration)
        {
            _dataSource = configuration.GetConfigurationSection<DataSource>();

            var absoluteFilePath = Path.Combine(_dataSource.Xml.FilePath, _dataSource.Xml.FileName);
            if (!File.Exists(absoluteFilePath))
                throw new Exception($"{Path.GetFileName(absoluteFilePath)}.{Path.GetExtension(absoluteFilePath)} does not exists.");

            LoadInMemory(absoluteFilePath);
        }
        #endregion

        #region Public Methods
        public IEnumerable<T> Read() => _items;

        public bool Write() => throw new NotImplementedException();
        #endregion

        #region Private Methods
        private void LoadInMemory(string absoluteFilePath)
            => _items = XDocument
                .Load(absoluteFilePath)
                .Descendants(GetXmlRootAttribute<T>())
                .Select(xElement => GetEntity(xElement))
                .ToList();

        private T GetEntity(XElement xElement)
        {
            var entity = new T();
            entity.Bind(xElement);

            return entity;
        }
        #endregion
    }
}
