namespace KeyEngine.Serialization.Utils
{
    public static class SerializationUtils
    {
        extension (object? obj)
        {
            public SerializableVariableType GetSerializableType()
            {
                if (obj == null)
                    return SerializableVariableType.Null;
                else if (obj is ISerializableEmitter)
                    return SerializableVariableType.EmitterSerializable;
                else if (obj.GetType().IsEnum)
                    return SerializableVariableType.Enum;

                return (SerializableVariableType)Type.GetTypeCode(obj.GetType());
            }
        }

        extension (Type? type)
        {
            public SerializableVariableType GetSerializableType()
            {
                if (type == null)
                    return SerializableVariableType.Null;
                else if (type is ISerializableEmitter)
                    return SerializableVariableType.EmitterSerializable;
                else if (type.IsEnum)
                    return SerializableVariableType.Enum;

                    return (SerializableVariableType)Type.GetTypeCode(type);
            }
        }

        extension (SerializableVariableType type)
        {
            public Type GetSystemType()
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
                    _ => throw new ArgumentException(type.ToString())
                };
            }
        }
    }
}
