using YamlDotNet.Core;
using System.Collections;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using KeyEngine.Serialization.Utils;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace KeyEngine.Serialization
{
    // GetSystemType возвращает object для типа Serializable. Нужно записывать кастомные Type по одному разу (Type.FullName) в начале каждого файла
    // При чтении, парсить Type и заносить в массив и после обращаться по индексу, чтобы файл весил меньше и Type.Parse вызывался меньше
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

                if (pair.Value.CollectionType == SerializableCollectionType.Dictionary && pair.Value is YamlDictionaryVariable dictionaryVariable)
                {
                    emitter.Emit(new Scalar("KeyType"));

                    emitter.Emit(new Scalar(dictionaryVariable.KeyType.ToString()));
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
            if (dataPair.CollectionType == SerializableCollectionType.None)
            {
                WriteVariableValue(ref emitter, ref objectSerializer, dataPair);
            }
            else
            {
                WriteCollection(ref emitter, ref objectSerializer, dataPair);
            }
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

            if (dataPair.VariableValue == null)
            {
                emitter.Emit(new SequenceEnd());
                return;
            }

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

            Type listType = typeof(List<>).MakeGenericType(variableType.GetSystemType());
            IList collectionValues = (IList)Activator.CreateInstance(listType)!;

            Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(dictionaryKeyType.GetSystemType(), variableType.GetSystemType());
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

            // Мб удалить, потому что если коллекцию уже создали но она пустая, то при десериализации будут ожидать, что она все так-же будет пустой а не null
            if (collectionValues.Count == 0 || dictionaryValues.Count == 0)
                return default;

            switch (collectionType)
            {
                case SerializableCollectionType.Array:

                    Array arrayValues = Array.CreateInstance(variableType.GetSystemType(), collectionValues.Count);

                    int i = 0;
                    foreach (var collectionValue in collectionValues)
                    {
                        arrayValues.SetValue(collectionValue, i);
                        i++;
                    }

                    result = arrayValues;
                    break;
                case SerializableCollectionType.List:

                    if (collectionValues.Count == 0)
                    {
                        result = default;
                        break;
                    }

                    result = collectionValues;
                    break;
                case SerializableCollectionType.Dictionary:

                    if (dictionaryValues.Count == 0)
                    {
                        result = default;
                        break;
                    }

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
                case SerializableVariableType.Serializable:
                    objectSerializer.Invoke(value, typeof(SerializeData));
                    break;
                case SerializableVariableType.Null:
                    emitter.Emit(new Scalar("null"));
                    break;
            }
        }

        private static void WriteVariableValue(ref IEmitter emitter, ref ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            WriteVariableValue(ref emitter, ref objectSerializer, dataPair.VariableType, dataPair.VariableValue);
        }

        private static object? ReadVariableValue(ref IParser parser, ref ObjectDeserializer objectDeserializer, SerializableVariableType variableType)
        {
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
                SerializableVariableType.Serializable => objectDeserializer.Invoke(typeof(SerializeData)),
                SerializableVariableType.Null => parser.Consume<Scalar>(),
                _ => null,
            };
        }
    }
}
