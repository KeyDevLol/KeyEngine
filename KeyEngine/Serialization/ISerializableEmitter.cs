using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace KeyEngine.Serialization
{
    public interface ISerializableEmitter
    {
        public void WriteData(ref IEmitter emitter, ref ObjectSerializer serializer, object? value);

        public static abstract object? ReadData(ref IParser parser, ref ObjectDeserializer deserializer);
    }
}
