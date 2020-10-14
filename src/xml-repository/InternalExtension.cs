namespace XmlRepository
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using XmlRepository.Base;
    using XmlRepository.Configuration;

    internal static class InternalExtension
    {
        internal static bool IsClass(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetType().IsClass;
        }

        internal static bool IsStringType(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetType() == typeof(string);
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesInfo(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetType().GetProperties();
        }

        internal static IEnumerable<string> GetPropertyNames(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetPropertiesInfo().Select(_ => _.Name);
        }

        internal static object? GetPropertyValue(this object entity, string propertyName)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetType().GetProperty(propertyName).GetValue(entity);
        }

        internal static string GetTypeName(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return entity.GetType().Name;
        }

        internal static string GetTypeNameInLower(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            return GetTypeName(entity).ToLower();
        }

        internal static XElement GetXElement(this object entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var propertyNames = entity.GetPropertyNames();
            if (!propertyNames.Any())
                return entity.GetXElement(entity);

            var xElements = new List<XElement>();
            foreach (var name in propertyNames)
            {
                var propertyValue = entity.GetPropertyValue(name);
                if (propertyValue.IsClass() && !propertyValue.IsStringType())
                    xElements.Add(GetXElement(propertyValue));                  //recursion
                else
                    xElements.Add(GetXElement(name.ToLower(), propertyValue));
            }

            return entity.GetXElement(xElements);
        }

        internal static XElement GetXElement(this object entity, object content)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            var xElement = new XElement(entity.GetTypeNameInLower(), content);
            return xElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static T GetConfigurationSection<T>(this IConfiguration configuration)
            => configuration.GetSection(typeof(T).Name).Get<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xElement"></param>
        internal static void Bind<T>(this T entity, XElement xElement) where T : BaseEntity
        {
            BindAttributes(entity, xElement);
            BindElements(entity, xElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xElement"></param>
        private static void BindAttributes<T>(this T entity, XElement xElement) where T : BaseEntity
        {
            var map = GetXmlAttributeAttributePropertyInfoMap(entity);
            foreach (var kvp in map)
            {
                var value = Convert.ChangeType(xElement.Attribute(kvp.Key).Value, kvp.Value.PropertyType);
                kvp.Value.SetValue(entity, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xElement"></param>
        private static void BindElements<T>(this T entity, XElement xElement) where T : BaseEntity
        {
            var map = entity.GetXmlElementAttributePropertyInfoMap();
            foreach (var kvp in map)
            {
                var value = Convert.ChangeType(xElement.Element(kvp.Key).Value, kvp.Value.PropertyType);
                kvp.Value.SetValue(entity, value);
            }
        }

        /// <summary>
        /// Gets the value of XmlRootAttribute when placed over a class.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns></returns>
        internal static string GetXmlRootAttribute<T>() where T : BaseEntity
        {
            var attributes = Attribute.GetCustomAttributes(typeof(T));
            foreach (var attr in attributes)
            {
                if (attr is XmlRootAttribute rootAttribute)
                    return rootAttribute.ElementName;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static IDictionary<string, PropertyInfo> GetXmlAttributeAttributePropertyInfoMap<T>(this T entity) where T : BaseEntity
        {
            var attributeAttributePropertyMap = new Dictionary<string, PropertyInfo>();
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is XmlAttributeAttribute xmlAttributeAttribute)
                        attributeAttributePropertyMap.Add(xmlAttributeAttribute.AttributeName, propertyInfo);
                }
            }

            return attributeAttributePropertyMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static IDictionary<string, PropertyInfo> GetXmlElementAttributePropertyInfoMap<T>(this T entity) where T : BaseEntity
        {
            var elementAttributePropertyMap = new Dictionary<string, PropertyInfo>();
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is XmlElementAttribute xmlElementAttribute)
                        elementAttributePropertyMap.Add(xmlElementAttribute.ElementName, propertyInfo);
                }
            }

            return elementAttributePropertyMap;
        }

        /// <summary>
        /// TODO: Add other exceptions
        /// Gets fully qualified filepath with extension
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        internal static string GetFullFilePath(this DataSource dataSource)
        {
            if (dataSource is null)
                throw new ArgumentNullException(nameof(dataSource));

            return Path.Combine(dataSource.Xml.FilePath, dataSource.Xml.FileName);
        }

        internal static string GetFilePath(this DataSource dataSource)
        {
            if (dataSource is null)
                throw new ArgumentNullException(nameof(dataSource));

            return dataSource.Xml.FilePath;
        }

        internal static string GetFileName(this DataSource dataSource)
        {
            if (dataSource is null)
                throw new ArgumentNullException(nameof(dataSource));

            return dataSource.Xml.FileName;
        }
    }
}
