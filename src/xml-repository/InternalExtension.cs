﻿namespace XmlRepository
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using XmlRepository.Base;

    internal static class InternalExtension
    {
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
        internal static void Bind<T>(this T entity, XElement xElement) where T : XmlBaseEntity
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
        private static void BindAttributes<T>(this T entity, XElement xElement) where T : XmlBaseEntity
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
        private static void BindElements<T>(this T entity, XElement xElement) where T : XmlBaseEntity
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
        internal static string GetXmlRootAttribute<T>() where T : XmlBaseEntity
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
        internal static IDictionary<string, PropertyInfo> GetXmlAttributeAttributePropertyInfoMap<T>(this T entity) where T : XmlBaseEntity
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
        internal static IDictionary<string, PropertyInfo> GetXmlElementAttributePropertyInfoMap<T>(this T entity) where T : XmlBaseEntity
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
    }
}