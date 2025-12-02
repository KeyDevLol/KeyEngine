namespace KeyEngine.Serialization
{
    public interface ISerializable
    {
        public SerializeData EditorSerialize();
        public void EditorDeserialize(SerializeData data);
    }
}
