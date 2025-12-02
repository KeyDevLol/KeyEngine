using System;
using System.Collections;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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

                SerializableVariableType dictionaryKeyType = SerializableVariableType.Null;
                if (collectionType == SerializableCollectionType.Dictionary)
                {
                    parser.Consume<Scalar>(); // KeyType
                    dictionaryKeyType = (SerializableVariableType)rootDeserializer.Invoke(typeof(SerializableVariableType))!;
                }

                object? value = null;

                if (collectionType != SerializableCollectionType.None)
                {
                    Type listType = typeof(List<>).MakeGenericType(GetTypeFromVariableType(variableType));
                    IList collectionValues = (IList)Activator.CreateInstance(listType)!;

                    parser.Consume<Scalar>(); // Value
                    parser.Consume<SequenceStart>();

                    while (!parser.TryConsume<SequenceEnd>(out _))
                    {
                        switch (collectionType)
                        {
                            case SerializableCollectionType.Array:
                                collectionValues.Add(ReadVariableValue(variableType, rootDeserializer));
                                break;
                            case SerializableCollectionType.List:
                                collectionValues.Add(ReadVariableValue(variableType, rootDeserializer));
                                break;
                            case SerializableCollectionType.Dictionary:
                                collectionValues.Add(ReadVariableValue(dictionaryKeyType, rootDeserializer)); // Key
                                collectionValues.Add(ReadVariableValue(variableType, rootDeserializer)); // Value
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

                            value = arrayValues;
                            break;
                        case SerializableCollectionType.List:
                            value = collectionValues;
                            break;
                        case SerializableCollectionType.Dictionary:
                            object? dictKey = null;
                            bool readKey = true;

                            Type dictKeyType = GetTypeFromVariableType(dictionaryKeyType);
                            Type dictValueType = GetTypeFromVariableType(variableType);

                            Type dictType = typeof(Dictionary<,>).MakeGenericType(dictKeyType, dictValueType);
                            IDictionary dict = (IDictionary)Activator.CreateInstance(dictType)!;

                            foreach (object? obj in collectionValues)
                            {
                                if (readKey)
                                {
                                    dictKey = obj;
                                }
                                else
                                {
                                    dict!.Add(dictKey!, obj);
                                }

                                readKey = !readKey;
                            }

                            value = dict;

                            break;
                    }
                }
                else
                {
                    parser.Consume<Scalar>(); // Value
                    value = ReadVariableValue(variableType, rootDeserializer);
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
                WriteVariable(ref emitter, serializer, pair.Value);

                emitter.Emit(new MappingEnd());
            }

            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }

        private static void WriteVariable(ref IEmitter emitter, ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            if (dataPair.CollectionType != SerializableCollectionType.None)
                WriteCollection(ref emitter, objectSerializer, dataPair);
            else
                WriteVariableValue(ref emitter, objectSerializer, dataPair);
        }

        private static void WriteCollection(ref IEmitter emitter, ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));

            switch (dataPair.CollectionType)
            {
                case SerializableCollectionType.Array:
                    object[]? array = dataPair.VariableValue as object[];

                    foreach (object? obj in array)
                    {
                        dataPair.VariableValue = obj;
                        WriteVariableValue(ref emitter, objectSerializer, dataPair);
                    }
                    break;

                case SerializableCollectionType.List:
                    IList list = dataPair.VariableValue as IList ?? throw new NullReferenceException();

                    foreach (object? obj in list)
                    {
                        dataPair.VariableValue = obj;
                        WriteVariableValue(ref emitter, objectSerializer, dataPair);
                    }
                    break;

                case SerializableCollectionType.Dictionary:

                    IDictionary dictionary = dataPair.VariableValue as IDictionary ?? throw new InvalidCastException();

                    foreach (DictionaryEntry pair in dictionary)
                    {
                        dataPair.VariableValue = pair.Key;
                        WriteVariableValue(ref emitter, objectSerializer, dataPair);
                        dataPair.VariableValue = pair.Value;
                        WriteVariableValue(ref emitter, objectSerializer, dataPair);
                    }

                    break;
            }

            emitter.Emit(new SequenceEnd());
        }

        private static object? ReadVariableValue(SerializableVariableType variableType, ObjectDeserializer objectDeserializer)
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
                _ => null,
            };
        }

        private static void WriteVariableValue(ref IEmitter emitter, ObjectSerializer objectSerializer, YamlVariable dataPair)
        {
            if (dataPair.VariableValue == default)
            {
                emitter.Emit(new Scalar("null"));
                return;
            }

            switch (dataPair.VariableType)
            {
                case SerializableVariableType.Boolean:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(bool));
                    break;
                case SerializableVariableType.Char:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(char));
                    break;
                case SerializableVariableType.SByte:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(sbyte));
                    break;
                case SerializableVariableType.Byte:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(byte));
                    break;
                case SerializableVariableType.Short:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(short));
                    break;
                case SerializableVariableType.UShort:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(ushort));
                    break;
                case SerializableVariableType.Int:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(int));
                    break;
                case SerializableVariableType.Uint:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(uint));
                    break;
                case SerializableVariableType.Long:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(long));
                    break;
                case SerializableVariableType.ULong:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(ulong));
                    break;
                case SerializableVariableType.Float:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(float));
                    break;
                case SerializableVariableType.Double:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(double));
                    break;
                case SerializableVariableType.Decimal:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(decimal));
                    break;
                case SerializableVariableType.String:
                    objectSerializer.Invoke(dataPair.VariableValue, typeof(string));
                    break;
            }
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
        private SerializableVariableType GetSerializableVariableType(object obj) => (SerializableVariableType)Type.GetTypeCode(obj.GetType());
    }
}
