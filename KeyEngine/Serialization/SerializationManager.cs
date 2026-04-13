using YamlDotNet.Serialization;

namespace KeyEngine.Serialization
{
    // TODO: Сделать комментарии нормальные
    /// <summary>
    /// Сериализирует ISerializable или сырую SerializeData
    /// </summary>
    public static class SerializationManager
    {
        public readonly static IDeserializer DefaultDeserializer;
        public readonly static ISerializer DefaultSerializer;

        static SerializationManager()
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

        public static string Serialize(ISerializable serializable)
        {
            return DefaultSerializer.Serialize(serializable.Serialize());
        }

        public static void SerializeToFile(string path, ISerializable serializable)
        {
            File.WriteAllText(path, Serialize(serializable));
        }
        public static string Serialize(SerializeData serializeData)
        {
            return DefaultSerializer.Serialize(serializeData);
        }

        public static void SerializeToFile(string path, SerializeData serializeData)
        {
            File.WriteAllText(path, DefaultSerializer.Serialize(serializeData));
        }

        public static string Serialize(object? obj)
        {
            return DefaultSerializer.Serialize(obj);
        }

        public static SerializeData? Deserialize(string yaml)
        {
            return DefaultDeserializer.Deserialize<SerializeData>(yaml);
        }

        public static SerializeData? DeserializeFile(string path)
        {
            return Deserialize(File.ReadAllText(path));
        }
    }
}
