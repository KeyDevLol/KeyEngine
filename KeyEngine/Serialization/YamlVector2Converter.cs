using KeyEngine.Mathematics;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace KeyEngine.Serialization
{
    public class YamlVector2Converter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Vector2);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            parser.Consume<MappingStart>();
            parser.Consume<Scalar>();
            float x = float.Parse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture);
            parser.Consume<Scalar>();
            float y = float.Parse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture);
            parser.Consume<MappingEnd>();

            return new Vector2(x, y);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            Vector2 vec = (Vector2)value!;

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("X"));
            emitter.Emit(new Scalar(vec.X.ToString(CultureInfo.InvariantCulture)));
            emitter.Emit(new Scalar("Y"));
            emitter.Emit(new Scalar(vec.Y.ToString(CultureInfo.InvariantCulture)));
            emitter.Emit(new MappingEnd());
        }
    }
}
