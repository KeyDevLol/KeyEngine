using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace KeyEngine.Serialization
{
    internal class YamlSceneConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Entity);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            emitter.Emit(new MappingStart());

        }
    }
}
