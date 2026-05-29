namespace KeyEngine.Serialization.Serializers
{
    public interface ISerializer
    {
        string Serialize(SerializeData serializeData);
        SerializeData Deserialize(string content);
    }
}
