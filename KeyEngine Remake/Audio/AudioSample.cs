using KeyEngine.Editor;
using OpenTK.Audio.OpenAL;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace KeyEngine.Audio
{
    // TODO: Потестировать работу полной версии NAudio на других платформах кроме Windows (Полная версия нужна для загрузки mp3)
    public class AudioSample : IAsset, IDisposable
    {
        public int BufferHandle { get; private set; } = -1;
        private IntPtr pointer;

        private bool disposed;

        public bool Loaded => BufferHandle != -1 && pointer != IntPtr.Zero;
        public bool AssetLoaded => throw new NotImplementedException();

        public AudioSample() { }
        public AudioSample(string filePath) => LoadWavFile(filePath);

        /// <summary>
        /// Loads audio file in wav format
        /// </summary>
        /// <param name="filePath">Path to audio file</param>
        public void LoadWavFile(string path)
        {
            //var l = new Mp3FileReaderBase.FrameDecompressorBuilder(;
            using (WaveFileReader waveFileReader = new WaveFileReader(path))
            {
                WaveStream stream = waveFileReader;

                int channels = stream.WaveFormat.Channels;
                int bits = stream.WaveFormat.BitsPerSample;
                int sampleRate = stream.WaveFormat.SampleRate;

                //int bufferSize = waveFileReader.;

                //int lol = waveFileReader.ReadByte();
                byte[] array;

                if (channels == 1)
                {
                    array = new byte[waveFileReader.Length];
                    waveFileReader.Read(array, 0, array.Length);
                }
                else
                {
                    var l = new StereoToMonoProvider16(waveFileReader);
                    array = new byte[waveFileReader.Length / 2];
                    l.Read(array, 0, array.Length);
                    channels = 1;
                }

                //Log.Print("Samples: " + waveFileReader.SampleCount);
                //Log.Print($"Channels: {channels}");
                //Log.Print($"Bits: {bits}");
                //Log.Print($"SampleRate: {sampleRate}");
                //Log.Print($"Buf size: {array.Length}");
                //Log.Print("Extra Size: " + waveFileReader.WaveFormat.ExtraSize);
                //Log.Print("BitsPerSample: " + waveFileReader.WaveFormat.BitsPerSample);
                //Log.Print("BlockAlign: " + waveFileReader.WaveFormat.BlockAlign);
                //Log.Print("Encoding: " + waveFileReader.WaveFormat.Encoding);
                //Log.Print($"NAUDIO END");

                pointer = GetDataPointer(array);
                BufferHandle = AL.GenBuffer();

                AL.BufferData(
                BufferHandle
                , AudioManager.GetSoundFormat(channels, bits)
                , pointer
                , array.Length
                , sampleRate);
            }

            return;

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                //if (format_signature != "fmt ")
                //    throw new NotSupportedException("Specified wave file is not supported.");

                int formatChunkSize = reader.ReadInt32();
                int audioFormat = reader.ReadInt16();
                int channels = reader.ReadInt16();
                int sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                int blockAlign = reader.ReadInt16();
                int bits = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));

                int data_chunk_size = reader.ReadInt32();

                //Init
                byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
                int bufferSize = data.Length;

                DisposeUnmanaged();

                pointer = GetDataPointer(data);

                BufferHandle = AL.GenBuffer();

                AL.BufferData(
                BufferHandle
                , AudioManager.GetSoundFormat(channels, bits)
                , pointer
                , bufferSize
                , sampleRate);

                Log.Print(data.Length);
                Log.Print($"Channels: {channels}");
                Log.Print($"Bits: {bits}");
                Log.Print($"SampleRate: {sampleRate}");
                Log.Print($"Buf size: {bufferSize}");
                Log.Print($"CUSTOM END");
            }
        }

        private IntPtr GetDataPointer(byte[] data)
        {
            if (AudioManager.UseUnsafeCode == true)
            {
                unsafe
                {
                    fixed (byte* p = data)
                    {
                        return (IntPtr)p;
                    }
                }
            }
            else
            {
                IntPtr result = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, result, data.Length);
                return result;
            }
        }
        private void DisposeUnmanaged()
        {
            if (Loaded)
                AL.DeleteBuffer(BufferHandle);

            if (pointer != IntPtr.Zero)
                Marshal.FreeHGlobal(pointer);
        }

        public void LoadAsset(string path)
        {
            LoadWavFile(path);
        }

        public void UnloadAsset()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
