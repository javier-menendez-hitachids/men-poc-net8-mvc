using System.Xml.Serialization;

namespace MenulioPocMvc.Models.Apis
{
    public class ApiEnum
    {
        [XmlElement("Item")]
        public string item { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
