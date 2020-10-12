namespace XmlRepository.Dto
{
    using System;
    using System.Xml.Serialization;
    using XmlRepository.Base;
    using static XmlRepository.Constant.Book;

    [XmlRoot(XmlRoot)]
    public class Book : BaseEntity
    {
        [XmlElement(XmlElement_Author)]
        public string Author { get; set; }

        [XmlElement(XmlElement_Title)]
        public string Title { get; set; }

        [XmlElement(XmlElement_Genre)]
        public string Genre { get; set; }

        [XmlElement(XmlElement_Price)]
        public decimal Price { get; set; }

        [XmlElement(XmlElement_PublishedOn)]
        public DateTime PublishedOn { get; set; }

        [XmlElement(XmlElement_Description)]
        public string Description { get; set; }
    }
}
