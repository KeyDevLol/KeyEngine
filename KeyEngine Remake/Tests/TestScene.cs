using KeyEngine.Graphics;
using KeyEngine.Editor.Serialization;
using System.Diagnostics;
using KeyEngine.Rendering;
using KeyEngine.Editor;
using KeyEngine.Audio;
using KeyEngine.Tests;

namespace KeyEngine 
{
    //
    public class TestScene : IScene
    {
        //private AssetReference<AudioSample> audioSample = new AssetReference<AudioSample>(@"Assets/Audio/Vigilantism 2.wav");

        public void Load()
        {
            Entity grass = ECS.AddEntity("Grass");
            grass.Layer = -1;
            SpriteRenderer spriteRenderer = grass.AddComponent<SpriteRenderer>();
            spriteRenderer.Color = Color.Green;
            grass.Scale = new Vector2(200, 200);

            Entity lol = ECS.AddEntity("Lol");
            lol.Scale = new Vector2(3, 3);
            lol.AddComponent<SpriteRenderer>();

            Entity audioListener = ECS.AddEntity("Audio Listener (Player)");
            audioListener.AddComponent<AudioListener>();
            audioListener.AddComponent<SpriteRenderer>();
            audioListener.AddComponent<Player>();

            //AudioSource audSource = lol.AddComponent<AudioSource>();
            //audSource.Looping = true;
            //audSource.PanSmoothness = 3.5f;
            //audSource.MaxDistance = 20;
            //audSource.Volume = 100;
            //audSource.SetAudioSample(audioSample.Value);
            //audSource.Play();
            ////for (int i = 0; i < 100; i++)
            ////{

            ////}

            //ComponentDatabase.Lol();
            ////Serialize();

            ////ECS.DeleteAllEntities();

            ////Deserialize();
        }

        public static void Serialize()
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

        public static void Deserialize()
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

        public static object? ReadByCode(int code, ref BinaryReader writer)
        {
            switch (code)
            {
                case 3:  return writer.ReadBoolean();
                case 4:  return writer.ReadChar(); 
                case 5:  return writer.ReadSByte();
                case 6:  return writer.ReadByte(); 
                case 7:  return writer.ReadInt16();
                case 8:  return writer.ReadUInt16();
                case 9:  return writer.ReadInt32();
                case 10: return writer.ReadUInt32(); 
                case 11: return writer.ReadInt64(); 
                case 12: return writer.ReadUInt64();
                case 13: return writer.ReadSingle();
                case 14: return writer.ReadDouble(); 
                case 15: return writer.ReadDecimal();
                case 18: return writer.ReadString();
                default: return null;
            }
        }

        public static void WriteByCode(int code, object value, ref BinaryWriter writer)
        {
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

        public void Unload()
        {
            
        }
    }
}
