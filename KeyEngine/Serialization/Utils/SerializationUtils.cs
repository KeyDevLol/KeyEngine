
namespace KeyEngine.Serialization.Utils
{
    public static class SerializationUtils
    {
        extension (object? obj)
        {
            private SerializableVariableType GetSerializableType() => obj == null ? SerializableVariableType.Null : (SerializableVariableType)Type.GetTypeCode(obj.GetType());
        }

        extension (Type? type)
        {
            public SerializableVariableType ToSerializableType() => (SerializableVariableType)Type.GetTypeCode(type);
        }

        extension (SerializableVariableType type)
        {
            public Type ToSystemType()
            {
                return type switch
                {
                    SerializableVariableType.Boolean => typeof(bool),
                    SerializableVariableType.Char => typeof(char),
                    SerializableVariableType.SByte => typeof(sbyte),
                    SerializableVariableType.Byte => typeof(byte),
                    SerializableVariableType.Short => typeof(short),
                    SerializableVariableType.UShort => typeof(ushort),
                    SerializableVariableType.Int => typeof(int),
                    SerializableVariableType.Uint => typeof(uint),
                    SerializableVariableType.Long => typeof(long),
                    SerializableVariableType.ULong => typeof(ulong),
                    SerializableVariableType.Float => typeof(float),
                    SerializableVariableType.Double => typeof(double),
                    SerializableVariableType.Decimal => typeof(decimal),
                    SerializableVariableType.String => typeof(string),
                    _ => typeof(object)
                };
            }
        }
    }
}
