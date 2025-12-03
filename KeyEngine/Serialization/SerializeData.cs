using System;
using System.Collections;
using System.Collections.Generic;

namespace KeyEngine.Serialization
{
    public struct SerializeData
    {
        public Dictionary<string, YamlVariable> DataPairs;

        public SerializeData() 
        { 
            DataPairs = [];
        }

        public readonly void AddArray<T>(string key, T[] array)
        {
            DataPairs.Add(key, new(SerializableCollectionType.Array, GetSerializableVariableType(typeof(T)), array));
        }

        public readonly void AddList<T>(string key, IList<T> list)
        {
            DataPairs.Add(key, new(SerializableCollectionType.List, GetSerializableVariableType(typeof(T)), list));
        }

        public readonly void AddDictionary<TKey, TValue>(string key, IDictionary<TKey, TValue> dictionary)
        {
            DataPairs.Add(key, new(SerializableCollectionType.Dictionary, GetSerializableVariableType(typeof(TValue)), dictionary));
        }

        public readonly void AddData(string key, object? value)
        {
            if (value is IList || value is IDictionary || value is Array)
                throw new InvalidOperationException();

            SerializableVariableType typeCode = value != null ? (SerializableVariableType)Type.GetTypeCode(value.GetType()) : SerializableVariableType.Null;

            DataPairs.Add(key, new(SerializableCollectionType.None, typeCode, value));
        }

        public readonly void RemoveData(string key)
        {
            DataPairs.Remove(key);
        }

        public readonly T? GetData<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (!DataPairs.TryGetValue(key, out YamlVariable variable))
                throw new KeyNotFoundException($"Key '{key}' not found in SerializeData.");

            object? obj = variable.VariableValue;

            if (obj is not null and T)
                return (T)obj;
            else
                return default;
        }

        public readonly T? GetData<T>(string key, ref T? output)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (!DataPairs.TryGetValue(key, out YamlVariable variable))
                throw new KeyNotFoundException($"Key '{key}' not found in SerializeData.");

            object? obj = variable.VariableValue;

            if (obj is not null and T)
            {
                T result = (T)obj;
                output = result;
                return result;
            }
            else
            {
                output = default;
                return default;
            }
        }

        private readonly SerializableVariableType GetSerializableVariableType(Type? type) => (SerializableVariableType)Type.GetTypeCode(type);
        private readonly SerializableVariableType GetSerializableVariableType(object? obj) => obj == null ? SerializableVariableType.Null : (SerializableVariableType)Type.GetTypeCode(obj.GetType());
    }
}
