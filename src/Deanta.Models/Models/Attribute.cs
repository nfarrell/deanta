namespace Deanta.Models.Models
{
    public class Attribute
    {
        public Attribute()
        {
        }

        public Attribute(string key, string value, string valueType)
        {
            Key = key;
            Value = value;
            ValueType = valueType;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }
    }
}
