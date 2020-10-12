namespace XmlRepository.Base.Data
{
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using XmlRepository.Configuration;

    public class XmlDataSource<TEntity> : IDataSource<TEntity>
        where TEntity : BaseEntity
    {
        #region Private Fields
        private readonly DataSource _dataSource;
        private readonly List<string> _errors = new List<string>();
        private List<TEntity> _entities;
        #endregion

        #region Private Properties
        /// <summary>
        /// Converts single xml element node to the respective <see cref="TEntity"/> instance.
        /// Also, returns errors if conversion of any of the node fails.
        /// </summary>
        private Func<XElement, TEntity> Selector
        {
            get
            {
                return xNode =>
                {
                    var entity = JsonConvert.DeserializeObject<TEntity>(
                        JObject.Parse(JsonConvert.SerializeXNode(xNode))[typeof(TEntity).Name.ToLower().Trim()].ToString()
                        , new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                _errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            }
                        }) ?? default;

                    return entity;
                };
            }
        }
        #endregion

        #region Ctors
        public XmlDataSource(IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            _dataSource = configuration.GetSection(nameof(DataSource)).Get<DataSource>();
            LoadElementsInMemory();
        }
        #endregion

        #region Public Methods
        /// <summary> Writes the data to the XML file. </summary>
        /// <param name="entities"> The <see cref="TEntity"/>. </param>
        /// <returns> True, if the the data was successfully writtent to the file. False otherwise. </returns>
        public bool Commit(IEnumerable<TEntity> entities)
            => InternalCommit(entities);

        /// <summary> Reads the entities collection from memory. </summary>
        /// <returns>The instance of <see cref="SourceEntity{TEntity}"/>.</returns>
        public SourceEntity<TEntity> Read()
        {
            if (_entities is null)
                throw new XmlException($"Failed to read XML file: {_dataSource.GetFileName()} or it has no records.");

            return new SourceEntity<TEntity>(_entities, _errors, _entities.Count, _errors.Count);
        }
        #endregion

        #region Private Methods
        private bool InternalCommit(IEnumerable<TEntity> entities)
        {
            return true;
        }

        /// <summary> Loads all the xml elements from file into the memory. </summary>
        private void LoadElementsInMemory() => _entities = XElement.Load(_dataSource.GetFullFilePath())
            .Elements()
            .Select(Selector)
            .ToList();
        #endregion
    }
}
