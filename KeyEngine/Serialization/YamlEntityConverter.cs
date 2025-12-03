using KeyEngine.Mathematics;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace KeyEngine.Serialization
{
    public class YamlEntityConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Entity);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            Entity entity = new Entity();
            Component? component = null;
            SerializeData serializeData = new SerializeData();
            parser.Consume<MappingStart>();

            parser.Consume<Scalar>(); // Name
            string name = parser.Consume<Scalar>().Value; // Name
            entity.Name = name;

            parser.Consume<Scalar>(); // Active
            bool active = bool.Parse(parser.Consume<Scalar>().Value); // Active
            entity.Active = active;

            parser.Consume<Scalar>(); // Position
            Vector2 position = (Vector2)rootDeserializer.Invoke(typeof(Vector2))!;
            entity.Position = position;

            parser.Consume<Scalar>(); // Scale
            Vector2 scale = (Vector2)rootDeserializer.Invoke(typeof(Vector2))!;
            entity.Scale = scale;

            parser.Consume<Scalar>(); // Rotation
            float rotation = (float)rootDeserializer.Invoke(typeof(float))!;
            entity.Rotation = rotation;

            parser.Consume<Scalar>(); // Layer
            int layer = (int)rootDeserializer.Invoke(typeof(int))!;
            entity.Layer = layer;

            string l = parser.Consume<Scalar>().Value; // Components
            parser.Consume<SequenceStart>();

            while (!parser.TryConsume<SequenceEnd>(out _))
            {
                Type? componentType = Type.GetType(parser.Consume<Scalar>().Value) ?? throw new TypeUnloadedException();

                component = (Component)entity.AddComponent(componentType);

                if (parser.TryConsume<Scalar>(out Scalar? empty))
                {
                    if (empty.Value == "Empty")
                        continue;
                }

                serializeData = (SerializeData)rootDeserializer.Invoke(typeof(SerializeData))!;
            }

            parser.Consume<MappingEnd>();

            if (component == null)
                return entity;

            component.EditorDeserialize(serializeData);

            return entity;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            Entity? entity = (Entity?)value ?? throw new NullReferenceException();

            emitter.Emit(new MappingStart());

            emitter.Emit(new Scalar("Name"));
            emitter.Emit(new Scalar(null, null, entity.Name ?? string.Empty, ScalarStyle.DoubleQuoted, true, false)); // Name

            emitter.Emit(new Scalar("Active"));
            emitter.Emit(new Scalar(entity.Active.ToString()));

            emitter.Emit(new Scalar("Position"));
            serializer.Invoke(entity.Position, typeof(Vector2));

            emitter.Emit(new Scalar("Scale"));
            serializer.Invoke(entity.Scale, typeof(Vector2));

            emitter.Emit(new Scalar("Rotation"));
            emitter.Emit(new Scalar(entity.Rotation.ToString(CultureInfo.InvariantCulture)));

            emitter.Emit(new Scalar("Layer"));
            emitter.Emit(new Scalar(entity.Layer.ToString()));

            emitter.Emit(new Scalar("Components"));
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Any));

            foreach (Component component in entity.GetAllComponents())
            {
                // Mb Null
                emitter.Emit(new Scalar(component.GetType().FullName!));
                WriteComponentYaml(ref emitter, component, serializer);
            }

            emitter.Emit(new SequenceEnd());

            emitter.Emit(new MappingEnd());
        }

        private static void WriteComponentYaml(ref IEmitter emitter, Component component, ObjectSerializer serializer)
        {
            if (component is not ISerializable serializable)
            {
                emitter.Emit(new Scalar("Empty"));
                return;
            }
            SerializeData serializeData = serializable.EditorSerialize();
            serializer.Invoke(serializeData, typeof(SerializeData));
        }
    }
}
