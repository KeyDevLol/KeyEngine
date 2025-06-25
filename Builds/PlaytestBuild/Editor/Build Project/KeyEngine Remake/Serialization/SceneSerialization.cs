using KeyEngine.Editor;
using KeyEngine.Editor.SupportedTypes;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.Serialization
{
    public static class SceneSerialization
    {
        public static void Test()
        { 
            using (FileStream fileStream = File.Open("SerializationTests.dat", FileMode.Create))
            {
                Stream stream = SerializeComponent();
                stream.CopyTo(fileStream);
            }

            using (BinaryReader reader = new BinaryReader(File.Open("SerializationTests.dat", FileMode.Open)))
            {
                Log.Print(reader.ReadInt32());
                Log.Print(reader.ReadInt32());
                Log.Print(reader.ReadInt32());
                Log.Print(reader.ReadSingle());
            }

            //Supported.
        }

        private static void WriteVariable(ref BinaryWriter writer, in object value)
        {
        }

        public static void TestDeserialize()
        {
            using FileStream fileStream = File.Open("SerializationTests.dat", FileMode.Create);
            using BinaryReader binaryReader = new BinaryReader(fileStream);

            int count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string key = binaryReader.ReadString();
                bool isNull = binaryReader.ReadBoolean();
            }
        }

        public static void TestSerialize(ISceneSerializable serializable)
        {
            ObjectSerializationData data = serializable.SceneSerialize();

            using FileStream fileStream = File.Open("SerializationTests.dat", FileMode.Create);
            using BinaryWriter binaryWriter = new BinaryWriter(fileStream);

            IEnumerator keys = data.GetKeys();

            binaryWriter.Write(data.Data.Keys.Count);

            keys.Reset();

            while (keys.MoveNext())
            {
                string key = (string)keys.Current;
                object? value = data.GetData(key);

                binaryWriter.Write(key);
                binaryWriter.Write(value == null);

                if (value != null)
                {
                    switch (value)
                    {
                        case bool:
                            binaryWriter.Write((bool)value);
                            break;
                        case byte:
                            binaryWriter.Write((byte)value);
                            break;
                        case short:
                            binaryWriter.Write((short)value);
                            break;
                        case int:
                            binaryWriter.Write((int)value);
                            break;
                        case uint:
                            binaryWriter.Write((uint)value);
                            break;                   
                        case long:
                            binaryWriter.Write((long)value);
                            break;
                        case string:
                            binaryWriter.Write((string)value);
                            break;
                    }
                }
            }

        }

        public static Stream SerializeComponent()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true);

            binaryWriter.Write(50);
            binaryWriter.Write(100);
            binaryWriter.Write(200);
            binaryWriter.Write(200.252f);

            binaryWriter.Close();
            binaryWriter.Dispose();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
