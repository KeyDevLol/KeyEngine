using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyEngine.Serialization
{
    public static class SerializationManager
    {
        public class DataConverter : JsonConverter<SerializableData>
        {
            public override SerializableData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                reader.Skip();
                SerializableData serializableData = new SerializableData();
                TypeCode typeCode = (TypeCode)reader.GetByte();

                serializableData.TypeCode = typeCode;

                switch (typeCode)
                {
                    case TypeCode.Byte: serializableData.Data = reader.GetByte(); break;
                    case TypeCode.SByte: serializableData.Data = reader.GetSByte(); break;
                    case TypeCode.Boolean: serializableData.Data = reader.GetBoolean(); break;
                    case TypeCode.Char: string? str = reader.GetString(); serializableData.Data = str?[0]; break;

                    case TypeCode.Int16: serializableData.Data = reader.GetInt16(); break;
                    case TypeCode.Int32: serializableData.Data = reader.GetInt32(); break;
                    case TypeCode.Int64: serializableData.Data = reader.GetInt64(); break;

                    case TypeCode.UInt16: serializableData.Data = reader.GetUInt16(); break;
                    case TypeCode.UInt32: serializableData.Data = reader.GetUInt32(); break;
                    case TypeCode.UInt64: serializableData.Data = reader.GetUInt64(); break;

                    case TypeCode.Single: serializableData.Data = reader.GetSingle(); break;
                    case TypeCode.Double: serializableData.Data = reader.GetDouble(); break;
                }

                return serializableData;
            }

            public override void Write(Utf8JsonWriter writer, SerializableData value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("T", (byte)value.TypeCode);

                if (value.Data == null)
                {
                    writer.WriteNullValue();
                    return;
                }    

                switch (value.TypeCode)
                {
                    case TypeCode.Byte: writer.WriteNumberValue((int)value.Data); break;
                    case TypeCode.SByte: writer.WriteNumberValue((sbyte)value.Data); break;
                    case TypeCode.Boolean: writer.WriteBooleanValue((bool)value.Data); break;
                    case TypeCode.Char: writer.WriteStringValue((string)value.Data); break;

                    case TypeCode.Int16: writer.WriteNumberValue((short)value.Data); break;
                    case TypeCode.Int32: writer.WriteNumberValue((int)value.Data); break;
                    case TypeCode.Int64: writer.WriteNumberValue((long)value.Data); break;

                    case TypeCode.UInt16: writer.WriteNumberValue((ushort)value.Data); break;
                    case TypeCode.UInt32: writer.WriteNumberValue((uint)value.Data); break;
                    case TypeCode.UInt64: writer.WriteNumberValue((ulong)value.Data); break;

                    case TypeCode.Single: writer.WriteNumberValue((float)value.Data); break;
                    case TypeCode.Double: writer.WriteNumberValue((double)value.Data); break;
                }

                writer.WriteEndObject();
            }
        }

        public class SerializableEntity
        {
            public string Name { get; set; }
            public List<SerializableComponent> components = new List<SerializableComponent>();

            public SerializableEntity(Entity entity)
            {

            }
        }

        public class SerializableComponent
        {
            public List<SerializableVariable> variables = new List<SerializableVariable>();

            public SerializableComponent(Component component)
            {

            }
        }

        public class SerializableVariable
        {
            public SerializableData? Data { get; set; }
            public char l { get; set; } = 'S';

            //public SerializableVariable(object variable)
            //{
            //    TypeCode typeCode = Type.GetTypeCode(variable.GetType());

            //    if (typeCode == TypeCode.Object || typeCode == TypeCode.Empty || typeCode == TypeCode.DBNull)
            //        throw new ArgumentException("Unsupported variable type.", nameof(variable));

            //    //Log.Print(typeCode);
            //}
        }

        [JsonConverter(typeof(DataConverter))]
        public class SerializableData
        {
            public TypeCode TypeCode;
            public object? Data;
        }

        public static void SerializeSceneJson()
        {
            SerializableVariable dsd = new SerializableVariable();
            SerializableData data = new SerializableData();
            data.TypeCode = TypeCode.Int32;
            data.Data = 502;
            dsd.Data = data;

            string json = JsonSerializer.Serialize(dsd);

            Log.Print(json);

            SerializableVariable lol = JsonSerializer.Deserialize<SerializableVariable>(json);

            Log.Print(lol.Data.Data);
        }

        public static void SerializeScene()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BinaryWriter writer = new BinaryWriter(File.Open("Scene.dat", FileMode.Create));

            Entity[] entities = ECS.GetAllEntities();

            foreach (Entity entity in entities)
            {
                IEnumerable<Component> components = entity.GetAllComponents();

                writer.Write(entity.Name);
                writer.Write(components.Count());

                //Log.Print(entity.Name);
                // Log.Print(components.Count());

                foreach (Component component in components)
                {
                    //Log.Print(component.GetType());
                    int hash = component.GetType().GetHashCode();
                    //Log.Print(hash);
                    writer.Write(hash);

                    SerializeData serializeData = component.SceneSerialize();

                    if (serializeData != SerializeData.Empty)
                    {
                        writer.Write(serializeData.Data.Count);

                        foreach (var l in serializeData.Data)
                        {
                            //Log.Print(l.Value);
                            SerializeData.Pair pair = l.Value;

                            writer.Write(l.Key);
                            writer.Write(pair.IsCustomSerializable ? 'c' : 'd');

                            if (pair.IsCustomSerializable)
                            {
                                writer.Write(pair.Type.FullName);
                                pair.CallSerializeWrite(ref writer);
                            }
                            else
                            {
                                sbyte typeCode = (sbyte)Type.GetTypeCode(pair.Type);
                                writer.Write(typeCode);
                                WriteByCode(typeCode, pair.Instance, ref writer);
                            }
                        }
                    }
                    else
                    {
                        writer.Write(0);
                    }
                }
            }

            writer.Dispose();
            sw.Stop();
            Log.Print($"Сцена сохранена за: {sw.ElapsedMilliseconds}мс");
            Log.Print($"Всего сохранено сущностей: {ECS.GetAllEntities().Length}");
        }

        public static void DeserializeScene(string path)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BinaryReader binaryReader = new BinaryReader(File.OpenRead("Scene.dat"));

            int entityCount = 0;
            while (binaryReader.PeekChar() > 0)
            {
                entityCount++;
                string name = binaryReader.ReadString();
                int componentCount = binaryReader.ReadInt32();

                //Log.Print(name);
                //Log.Print(componentCount);

                Entity entity = ECS.AddEntity(name);

                for (int i = 0; i < componentCount; i++)
                {
                    int hash = binaryReader.ReadInt32();
                    //Log.Print(hash);
                    //Log.Print(ComponentDatabase.components[hash]);
                    Component? component = Activator.CreateInstance(ComponentDatabase.components[hash], [entity]) as Component ?? throw new NullReferenceException();
                    int serializeDataCount = binaryReader.ReadInt32();

                    //Log.Print(serializeDataCount);

                    entity.AddComponent(component);

                    if (serializeDataCount != 0)
                    {
                        SerializeData serializeData = new SerializeData();

                        for (int s = 0; s < serializeDataCount; s++)
                        {
                            string key = binaryReader.ReadString();
                            char serializeType = binaryReader.ReadChar();

                            if (serializeType == 'c')
                            {
                                //Log.Print("Read custom serializable");

                                Type? type = Type.GetType(binaryReader.ReadString());
                                object? instance = Activator.CreateInstance(type);
                                Log.Print(type);
                                Log.Print(instance);
                                SerializeData.Pair.SerializeReadMethod.Invoke(instance, [binaryReader]);

                                serializeData.AddData(key, instance);
                            }
                            else
                            {
                                //Log.Print("Read default serializable");

                                object? data = ReadByCode(binaryReader.ReadSByte(), ref binaryReader);
                                //Log.Print($"Read default data type {data.GetType()}");
                                serializeData.AddData(key, data);
                            }
                        }

                        component.SceneDeserialize(serializeData);
                    }
                }
            }

            binaryReader.Dispose();
            sw.Stop();
            Log.Print($"Сцена загружена за: {sw.ElapsedMilliseconds}мс");
            Log.Print($"Всего загружено сущностей: {entityCount}");
        }

        public static void SerializeEntity()
        {

        }

        public static object? ReadByCode(int code, ref BinaryReader writer)
        {
            return code switch
            {
                3 => writer.ReadBoolean(),
                4 => writer.ReadChar(),
                5 => writer.ReadSByte(),
                6 => writer.ReadByte(),
                7 => writer.ReadInt16(),
                8 => writer.ReadUInt16(),
                9 => writer.ReadInt32(),
                10 => writer.ReadUInt32(),
                11 => writer.ReadInt64(),
                12 => writer.ReadUInt64(),
                13 => writer.ReadSingle(),
                14 => writer.ReadDouble(),
                15 => writer.ReadDecimal(),
                18 => writer.ReadString(),
                _ => null,
            };
        }

        public static void WriteByCode(int code, object? value, ref BinaryWriter writer)
        {
            if (value == null)
                return;

            switch (code)
            {
                case 3: writer.Write((bool)value); break;
                case 4: writer.Write((char)value); break;
                case 5: writer.Write((sbyte)value); break;
                case 6: writer.Write((byte)value); break;
                case 7: writer.Write((short)value); break;
                case 8: writer.Write((ushort)value); break;
                case 9: writer.Write((int)value); break;
                case 10: writer.Write((uint)value); break;
                case 11: writer.Write((long)value); break;
                case 12: writer.Write((ulong)value); break;
                case 13: writer.Write((float)value); break;
                case 14: writer.Write((double)value); break;
                case 15: writer.Write((decimal)value); break;
                case 18: writer.Write((string)value); break;
            }
        }
    }
}
