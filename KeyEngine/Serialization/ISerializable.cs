namespace KeyEngine.Serialization
{
    public interface ISerializable
    {
        public SerializeData Serialize();
        public void Deserialize(SerializeData data);
    }
}
