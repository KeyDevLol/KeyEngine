using System.Collections;

namespace KeyEngine.Serialization
{
    public struct SerializeData
    {
        public Dictionary<string, YamlVariable> DataPairs;

        public SerializeData() 
        { 
            DataPairs = [];
        }

        public readonly void AddArray(string key, Array array)
        {
            SerializableVariableType typeCode = SerializableVariableType.Null;

            if (array.Length > 0)
            {
                IEnumerator enumerator = array.GetEnumerator();
                enumerator.MoveNext();
                SerializableVariableType keyType = GetSerializableVariableType(enumerator.Current);
                enumerator.Reset();

                typeCode = keyType;
            }

            DataPairs.Add(key, new(SerializableCollectionType.Array, typeCode, array));
        }

        public readonly void AddList(string key, IList list)
        {
            SerializableVariableType typeCode = SerializableVariableType.Null;

            if (list.Count > 0)
            {
                IEnumerator enumerator = list.GetEnumerator();
                enumerator.MoveNext();
                SerializableVariableType keyType = GetSerializableVariableType(enumerator.Current);
                enumerator.Reset();

                typeCode = keyType;
            }

            DataPairs.Add(key, new(SerializableCollectionType.List, typeCode, list));
        }

        public readonly void AddDictionary(string key, IDictionary dictionary)
        {
            SerializableVariableType typeCode = SerializableVariableType.Null;

            if (dictionary.Count > 0)
            {
                IEnumerator keysEnumerator = dictionary.Keys.GetEnumerator();
                IEnumerator valuesEnumerator = dictionary.Values.GetEnumerator();
                keysEnumerator.MoveNext();
                SerializableVariableType keyType = GetSerializableVariableType(keysEnumerator.Current);
                keysEnumerator.Reset();
                valuesEnumerator.MoveNext();
                SerializableVariableType valueType = GetSerializableVariableType(valuesEnumerator.Current);
                valuesEnumerator.Reset();

                typeCode = keyType;
                typeCode |= valueType;
            }

            DataPairs.Add(key, new(SerializableCollectionType.Dictionary, typeCode, dictionary));
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
            object? obj = DataPairs[key].VariableValue;

            if (obj != null)
                return (T)obj;
            else
                return default;
        }

        public readonly T? GetData<T>(string key, ref T? output)
        {
            if (!DataPairs.TryGetValue(key, out YamlVariable variable))
                throw new KeyNotFoundException();

            object? obj = variable.VariableValue;

            if (obj != null)
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

        private readonly SerializableVariableType GetSerializableVariableType(Type type) => (SerializableVariableType)Type.GetTypeCode(type);
        private readonly SerializableVariableType GetSerializableVariableType(object obj) => (SerializableVariableType)Type.GetTypeCode(obj.GetType());
    }
}
