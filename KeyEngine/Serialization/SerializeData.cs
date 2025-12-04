using System.Collections;
using KeyEngine.Serialization.Utils;

namespace KeyEngine.Serialization
{
    public class SerializeData
    {
        public Dictionary<string, YamlVariable> DataPairs = [];

        public void AddArray<T>(string key, T[] array)
        {
            DataPairs.Add(key, new(SerializableCollectionType.Array, typeof(T).GetSerializableType(), array));
        }

        public void AddList<T>(string key, IList<T> list)
        {
            DataPairs.Add(key, new(SerializableCollectionType.List, typeof(T).GetSerializableType(), list));
        }

        public void AddDictionary<TKey, TValue>(string key, IDictionary<TKey, TValue> dictionary)
        {
            DataPairs.Add(key, new YamlDictionaryVariable(typeof(TKey).GetSerializableType(), typeof(TValue).GetSerializableType(), dictionary));
        }

        public void AddData(string key, object? value)
        {
            if (value is IList || value is IDictionary || value is Array)
                throw new InvalidOperationException();

            SerializableVariableType typeCode = value != null ? (SerializableVariableType)Type.GetTypeCode(value.GetType()) : SerializableVariableType.Null;

            DataPairs.Add(key, new(SerializableCollectionType.None, typeCode, value));
        }

        public void RemoveData(string key)
        {
            DataPairs.Remove(key);
        }

        public T? GetData<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (!DataPairs.TryGetValue(key, out YamlVariable? variable))
                throw new KeyNotFoundException($"Key '{key}' not found in SerializeData.");

            object? obj = variable.VariableValue;
            Log.Print(obj);
            if (obj is not null and T)
                return (T)obj;
            else
            {

                return default;
            }
        }

        public T? GetData<T>(string key, ref T? output)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (!DataPairs.TryGetValue(key, out YamlVariable? variable))
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
    }
}
