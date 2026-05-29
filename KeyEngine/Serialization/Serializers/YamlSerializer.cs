using YamlDotNet.Serialization;
using YISerializer = YamlDotNet.Serialization.ISerializer;

namespace KeyEngine.Serialization.Serializers
{
    public class YamlSerializer : ISerializer
    {
        public readonly IDeserializer DefaultDeserializer;
        public readonly YISerializer DefaultSerializer;

        public YamlSerializer()
        {
            DefaultDeserializer = new DeserializerBuilder()
                .WithTypeConverter(new YamlSerializeDataConverter())
                .WithTypeConverter(new YamlEntityConverter())
                .WithTypeConverter(new YamlVector2Converter())
                .IgnoreUnmatchedProperties()
                .Build();

            DefaultSerializer = new SerializerBuilder()
                .WithTypeConverter(new YamlSerializeDataConverter())
                .WithTypeConverter(new YamlEntityConverter())
                .WithTypeConverter(new YamlVector2Converter())
                .DisableAliases()
                .Build();
        }

        public string Serialize(SerializeData serializeData)
        {
            return DefaultSerializer.Serialize(serializeData);
        }

        public SerializeData Deserialize(string content)
        {
            return DefaultDeserializer.Deserialize<SerializeData>(content);
        }
    }
}
