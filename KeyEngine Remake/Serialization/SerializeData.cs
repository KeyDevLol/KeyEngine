using KeyEngine.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.Serialization
{
    public readonly struct SerializeData : IEquatable<SerializeData>
    {
        public readonly Dictionary<string, Pair> Data = new Dictionary<string, Pair>();
        public bool IsEmpty => Data.Count == 0;

        public SerializeData() { }

        public void AddData(string key, object value)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            Data.Add(key, new Pair(value));
        }

        public object? GetData<T>(string key, ref T? outData)
        {
            ArgumentNullException.ThrowIfNull(key);
            object? data = Data[key].Instance;

            if (data != null)
                outData = (T)data;

            return data;
        }

        public bool TryGetData(string key, out object? data)
        {
            ArgumentNullException.ThrowIfNull(key);
            data = Data[key];
            return data != null;
        }

        public IEnumerator GetKeys()
        {
            return Data.Keys.GetEnumerator();
        }

        public IEnumerator GetValues()
        {
            return Data.Values.GetEnumerator();
        }

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

        public struct Pair
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
