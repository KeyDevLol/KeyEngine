using System;
using System.Collections;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace KeyEngine.Serialization
{
    public class YamlSerializeDataConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(SerializeData);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            SerializeData serializeData = new SerializeData();

            parser.Consume<MappingStart>();
            parser.Consume<Scalar>();
            parser.Consume<SequenceStart>();

            while (!parser.TryConsume<SequenceEnd>(out _))
            {
                parser.Consume<MappingStart>();

                parser.Consume<Scalar>(); // Key
                string pairKey = parser.Consume<Scalar>().Value;

                parser.Consume<Scalar>(); // CollectionType
                SerializableCollectionType collectionType = (SerializableCollectionType)rootDeserializer.Invoke(typeof(SerializableCollectionType))!; // CollectionType Value

                parser.Consume<Scalar>(); // VariableType
                SerializableVariableType variableType = (SerializableVariableType)rootDeserializer.Invoke(typeof(SerializableVariableType))!; // VariableType Value

                object? value = null;

                if (variableType != SerializableVariableType.Null)
                {
                    if (collectionType == SerializableCollectionType.None)
                        value = ReadVariable(ref parser, ref rootDeserializer, collectionType, variableType);
                    else
                        value = ReadCollection(ref parser, ref rootDeserializer, collectionType, variableType);
                }
                else
                {
                    parser.Consume<Scalar>(); // Null Value
                }

                serializeData.DataPairs.Add(pairKey, new YamlVariable(collectionType, variableType, value));

                parser.Consume<MappingEnd>();
            }

            parser.Consume<MappingEnd>();

            return serializeData;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            SerializeData data = (SerializeData)value!;

            emitter.Emit(new MappingStart()); 
            emitter.Emit(new Scalar("DataPairs"));
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));

            foreach (KeyValuePair<string, YamlVariable> pair in data.DataPairs)
            {
                emitter.Emit(new MappingStart());

                emitter.Emit(new Scalar("Key"));
                emitter.Emit(new Scalar(pair.Key));

                emitter.Emit(new Scalar("CollectionType"));
                emitter.Emit(new Scalar(pair.Value.CollectionType.ToString()));

                emitter.Emit(new Scalar("VariableType"));
                emitter.Emit(new Scalar(pair.Value.VariableType.ToString()));

                if (pair.Value.CollectionType == SerializableCollectionType.Dictionary && pair.Value.VariableValue is IDictionary dictionary)
                {
                    emitter.Emit(new Scalar("KeyType"));
                    SerializableVariableType keyType = SerializableVariableType.Null;

                    if (dictionary.Count > 0)
                    {
                        IEnumerator enumerator = dictionary.Keys.GetEnumerator();
                        enumerator.MoveNext();
                        keyType = GetSerializableVariableType(enumerator.Current);
                        enumerator.Reset();
                    }

                    emitter.Emit(new Scalar(keyType.ToString()));
                }

                emitter.Emit(new Scalar("Value"));
                WriteVariable(ref emitter, ref serializer, pair.Value);

                emitter.Emit(new MappingEnd());
            }

            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }

        private static void WriteVariable(ref IEmitter emitter, ref ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            if (dataPair.CollectionType != SerializableCollectionType.None)
                WriteCollection(ref emitter, ref objectSerializer, dataPair);
            else
                WriteVariableValue(ref emitter, ref objectSerializer, dataPair);
        }

        private static object? ReadVariable(ref IParser parser, ref ObjectDeserializer deserializer, SerializableCollectionType collectionType, SerializableVariableType variableType)
        {
            if (collectionType == SerializableCollectionType.None)
            {
                parser.Consume<Scalar>(); // Value
                return ReadVariableValue(ref parser, ref deserializer, variableType);
            }
            else
            {
                return ReadCollection(ref parser, ref deserializer, collectionType, variableType);
            }
        }

        private static void WriteCollection(ref IEmitter emitter, ref ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));

            switch (dataPair.CollectionType)
            {
                case SerializableCollectionType.Array:
                    object[]? array = (object[]?)dataPair.VariableValue;

                    if (array == null)
                    {
                        WriteVariableValue(ref emitter, ref objectSerializer, SerializableVariableType.Null, null);
                        break;
                    }

                    foreach (object? obj in array)
                    {
                        WriteVariableValue(ref emitter, ref objectSerializer, dataPair.VariableType, obj);
                    }
                    break;

                case SerializableCollectionType.List:
                    IList? list = dataPair.VariableValue as IList;

                    if (list == null)
                    {
                        WriteVariableValue(ref emitter, ref objectSerializer, SerializableVariableType.Null, null);
                        break;
                    }

                    foreach (object? obj in list)
                    {
                        WriteVariableValue(ref emitter, ref objectSerializer, dataPair.VariableType, obj);
                    }
                    break;

                case SerializableCollectionType.Dictionary:

                    IDictionary? dictionary = dataPair.VariableValue as IDictionary;

                    if (dictionary == null)
                    {
                        WriteVariableValue(ref emitter, ref objectSerializer, SerializableVariableType.Null, null);
                        break;
                    }

                    foreach (DictionaryEntry pair in dictionary)
                    {
                        dataPair.VariableValue = pair.Key;
                        WriteVariableValue(ref emitter, ref objectSerializer, dataPair);
                        dataPair.VariableValue = pair.Value;
                        WriteVariableValue(ref emitter, ref objectSerializer, dataPair);
                    }

                    break;
            }

            emitter.Emit(new SequenceEnd());
        }

        private static object? ReadCollection(ref IParser parser, ref ObjectDeserializer deserializer, SerializableCollectionType collectionType, SerializableVariableType variableType)
        {
            object? result = null;

            SerializableVariableType dictionaryKeyType = SerializableVariableType.Null;
            if (collectionType == SerializableCollectionType.Dictionary)
            {
                parser.Consume<Scalar>(); // KeyType
                dictionaryKeyType = (SerializableVariableType)deserializer.Invoke(typeof(SerializableVariableType))!;
            }

            Type listType = typeof(List<>).MakeGenericType(GetTypeFromVariableType(variableType));
            IList collectionValues = (IList)Activator.CreateInstance(listType)!;

            Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(GetTypeFromVariableType(dictionaryKeyType), GetTypeFromVariableType(variableType));
            IDictionary dictionaryValues = (IDictionary)Activator.CreateInstance(dictionaryType)!;

            parser.Consume<Scalar>(); // Value
            parser.Consume<SequenceStart>();

            while (!parser.TryConsume<SequenceEnd>(out _))
            {
                switch (collectionType)
                {
                    case SerializableCollectionType.Array:
                        collectionValues.Add(ReadVariableValue(ref parser, ref deserializer, variableType));
                        break;
                    case SerializableCollectionType.List:
                        collectionValues.Add(ReadVariableValue(ref parser, ref deserializer, variableType));
                        break;
                    case SerializableCollectionType.Dictionary:
                        object key = ReadVariableValue(ref parser, ref deserializer, dictionaryKeyType)!; // Key
                        object? value = ReadVariableValue(ref parser, ref deserializer, variableType); // Value
                        dictionaryValues.Add(key, value);
                        break;
                }
            }

            switch (collectionType)
            {
                case SerializableCollectionType.Array:
                    Array arrayValues = Array.CreateInstance(GetTypeFromVariableType(variableType), collectionValues.Count);

                    int i = 0;
                    foreach (var collectionValue in collectionValues)
                    {
                        arrayValues.SetValue(collectionValue, i);
                        i++;
                    }

                    result = arrayValues;
                    break;
                case SerializableCollectionType.List:
                    result = collectionValues;
                    break;
                case SerializableCollectionType.Dictionary:
                    result = dictionaryValues;
                    break;
            }

            return result;
        }

        private static void WriteVariableValue(ref IEmitter emitter, ref ObjectSerializer objectSerializer, SerializableVariableType variableType, object? value)
        {
            if (value == default)
            {
                emitter.Emit(new Scalar("null"));
                return;
            }

            switch (variableType)
            {
                case SerializableVariableType.Boolean:
                    objectSerializer.Invoke(value, typeof(bool));
                    break;
                case SerializableVariableType.Char:
                    objectSerializer.Invoke(value, typeof(char));
                    break;
                case SerializableVariableType.SByte:
                    objectSerializer.Invoke(value, typeof(sbyte));
                    break;
                case SerializableVariableType.Byte:
                    objectSerializer.Invoke(value, typeof(byte));
                    break;
                case SerializableVariableType.Short:
                    objectSerializer.Invoke(value, typeof(short));
                    break;
                case SerializableVariableType.UShort:
                    objectSerializer.Invoke(value, typeof(ushort));
                    break;
                case SerializableVariableType.Int:
                    objectSerializer.Invoke(value, typeof(int));
                    break;
                case SerializableVariableType.Uint:
                    objectSerializer.Invoke(value, typeof(uint));
                    break;
                case SerializableVariableType.Long:
                    objectSerializer.Invoke(value, typeof(long));
                    break;
                case SerializableVariableType.ULong:
                    objectSerializer.Invoke(value, typeof(ulong));
                    break;
                case SerializableVariableType.Float:
                    objectSerializer.Invoke(value, typeof(float));
                    break;
                case SerializableVariableType.Double:
                    objectSerializer.Invoke(value, typeof(double));
                    break;
                case SerializableVariableType.Decimal:
                    objectSerializer.Invoke(value, typeof(decimal));
                    break;
                case SerializableVariableType.String:
                    objectSerializer.Invoke(value, typeof(string));
                    break;
            }
        }

        private static void WriteVariableValue(ref IEmitter emitter, ref ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            WriteVariableValue(ref emitter, ref objectSerializer, dataPair.VariableType, dataPair.VariableValue);
        }

        private static object? ReadVariableValue(ref IParser parser, ref ObjectDeserializer objectDeserializer, SerializableVariableType variableType)
        {
            if (variableType == SerializableVariableType.Serializable)
                throw new InvalidOperationException("Attempt to read a Serializable type as a primitive type.");

            return variableType switch
            {
                SerializableVariableType.Boolean => objectDeserializer.Invoke(typeof(bool)),
                SerializableVariableType.Char => objectDeserializer.Invoke(typeof(char)),
                SerializableVariableType.SByte => objectDeserializer.Invoke(typeof(sbyte)),
                SerializableVariableType.Byte => objectDeserializer.Invoke(typeof(byte)),
                SerializableVariableType.Short => objectDeserializer.Invoke(typeof(short)),
                SerializableVariableType.UShort => objectDeserializer.Invoke(typeof(ushort)),
                SerializableVariableType.Int => objectDeserializer.Invoke(typeof(int)),
                SerializableVariableType.Uint => objectDeserializer.Invoke(typeof(uint)),
                SerializableVariableType.Long => objectDeserializer.Invoke(typeof(long)),
                SerializableVariableType.ULong => objectDeserializer.Invoke(typeof(ulong)),
                SerializableVariableType.Float => objectDeserializer.Invoke(typeof(float)),
                SerializableVariableType.Double => objectDeserializer.Invoke(typeof(double)),
                SerializableVariableType.Decimal => objectDeserializer.Invoke(typeof(decimal)),
                SerializableVariableType.String => objectDeserializer.Invoke(typeof(string)),
                SerializableVariableType.Null => parser.Consume<Scalar>(),
                _ => null,
            };
        }

        private static Type GetTypeFromVariableType(SerializableVariableType variableType)
        {
            return variableType switch
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

        private SerializableVariableType GetSerializableVariableType(Type type) => (SerializableVariableType)Type.GetTypeCode(type);
        private SerializableVariableType GetSerializableVariableType(object obj) => obj == null ? SerializableVariableType.Null : (SerializableVariableType)Type.GetTypeCode(obj.GetType());
    }
}
