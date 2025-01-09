using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MenulioPocMvc.Models.Common.ExtensionMethods
{
    public static class XmlExtensions
    {
        public static void AddAttr(this XElement elem, string attrName, string value)
        {
            if (value == null)
                return;

            elem.Add(new XAttribute(attrName, value));
        }

        public static void AddAttr(this XElement elem, string attrName, DateTime? value)
        {
            if (!value.HasValue)
                return;

            elem.Add(new XAttribute(attrName, value.Value));
        }

        public static void AddAttr(this XElement elem, string attrName, Guid? value)
        {
            if (!value.HasValue)
                return;

            elem.Add(new XAttribute(attrName, value.Value.ToString()));
        }

        public static void AddAttr(this XElement elem, string attrName, int? value)
        {
            if (!value.HasValue)
                return;

            elem.Add(new XAttribute(attrName, value.Value.ToString(CultureInfo.InvariantCulture)));
        }

        public static void AddAttr(this XElement elem, string attrName, bool value)
        {
            elem.Add(new XAttribute(attrName, value ? "true" : "false"));
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseNode(this XElement node, string collection)
        {
            var child = node.Element(collection);
            if (child == null)
                return new List<KeyValuePair<string, string>>();

            return child.Elements().Select(i => new KeyValuePair<string, string>(i.Attribute("type").Value, i.Value));
        }

        public static XElement ToItemsNode(this IDictionary<string, string> items, string collection)
        {
            if (items.Count == 0)
                return null;

            var xElement = new XElement(collection);
            foreach (var itemElement in items.Select(item => new XElement("item", new XAttribute("type", item.Key), item.Value)))
            {
                xElement.Add(itemElement);
            }

            return xElement;
        }

        public static string ToXmlDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToXmlDateFormat(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return ToXmlDateFormat(dateTime.Value);
        }

        public static string ToXmlDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string ToXmlDateTimeFormat(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return ToXmlDateTimeFormat(dateTime.Value);
        }
    }
}
