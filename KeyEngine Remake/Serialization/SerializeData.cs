using System.Reflection;
using System.Collections;

namespace KeyEngine.Serialization
{
    public readonly struct SerializeData : IEquatable<SerializeData>
    {
        public readonly Dictionary<string, Pair> Data = new Dictionary<string, Pair>();
        public bool IsEmpty => Data.Count == 0;

        public SerializeData() { }

        public void AddData(string key, object? value)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            Data.Add(key, new Pair(value));
        }

        public void GetData<T>(string key, ref T? outData)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            object? data = Data[key].Instance;

            if (data != null)
                outData = (T)data;
            else
                outData = default;
        }

        public T? GetData<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            object? data = Data[key].Instance;

            if (data != null)
                return (T)data;
            else
                return default;
        }

        public bool TryGetData(string key, out object? data)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (Data.TryGetValue(key, out Pair value))
            {
                data = value;
                return true;
            }

            data = Data[key];
            return false;
        }

        public IEnumerator GetKeys() => Data.Keys.GetEnumerator();

        public IEnumerator GetValues() => Data.Values.GetEnumerator();

        public bool Equals(SerializeData other)
        {
            return Data == other.Data;
        }

        public static bool operator ==(SerializeData left, SerializeData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SerializeData left, SerializeData right)
        {
            return !left.Equals(right);
        }

        public static readonly SerializeData Empty = new SerializeData();

        public override bool Equals(object? obj)
        {
            return obj is SerializeData data && Equals(data);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        public readonly struct Pair
        {
            public readonly Type? Type;
            public readonly object? Instance;
            public readonly bool IsCustomSerializable;
            public readonly bool IsNull;

            public static readonly MethodInfo? SerializeWriteMethod = typeof(ISerializable).GetMethod(nameof(ISerializable.SerializeWrite));
            public static readonly MethodInfo? SerializeReadMethod = typeof(ISerializable).GetMethod(nameof(ISerializable.SerializeRead));

            public Pair(object? instance)
            {
                Instance = instance;

                if (instance == null)
                {
                    IsNull = true;  
                    return;
                }

                Type = instance.GetType();
                Type? interfaceType = Type.GetInterface(nameof(ISerializable));
                IsCustomSerializable = interfaceType != null;
            }

            public void CallSerializeWrite(ref BinaryWriter writer)
            {
                if (SerializeWriteMethod == null)
                    return;

                SerializeWriteMethod.Invoke(Instance, [writer]);
            }

            public void CallSerializeRead(ref BinaryReader reader)
            {
                if (SerializeReadMethod == null)
                    return;

                SerializeReadMethod.Invoke(Instance, [reader]);
            }
        }
    }
}
