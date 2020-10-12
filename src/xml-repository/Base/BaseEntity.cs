namespace XmlRepository.Base
{
    using System;
    using System.Xml.Serialization;
    using static XmlRepository.Constant.XmlBaseEntity;

    [Serializable]
    public class BaseEntity
    {
        [XmlAttribute(XmlAttribute_Id)]
        public string Id { get; set; }
    }
}
