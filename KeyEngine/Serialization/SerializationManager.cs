using KeyEngine.Serialization.Serializers;

namespace KeyEngine.Serialization
{
    // TODO: Сделать комментарии нормальные
    /// <summary>
    /// Сериализирует ISerializable или сырую SerializeData
    /// </summary>
    public static class SerializationManager
    {
        public readonly static ISerializer Serializer;

        static SerializationManager()
        {
            Serializer = new YamlSerializer();
        }

        public static string Serialize(SerializeData serializeData)
        {
            return Serializer.Serialize(serializeData);
        }

        public static string Serialize(ISerializable serializable)
        {
            return Serializer.Serialize(serializable.Serialize());
        }

        public static void SerializeToFile(string path, SerializeData serializeData)
        {
            File.WriteAllText(path, Serializer.Serialize(serializeData));
        }

        public static void SerializeToFile(string path, ISerializable serializable)
        {
            File.WriteAllText(path, Serialize(serializable));
        }

        public static SerializeData? Deserialize(string yaml)
        {
            return Serializer.Deserialize(yaml);
        }

        public static SerializeData? DeserializeFile(string path)
        {
            return Deserialize(File.ReadAllText(path));
        }
    }
}
