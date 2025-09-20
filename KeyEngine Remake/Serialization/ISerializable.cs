namespace KeyEngine.Serialization
{
    public interface ISerializable
    {
        public void SerializeWrite(ref BinaryWriter writer);
        public void SerializeRead(ref BinaryReader reader);
    }
}
