using NLayer;
using NAudio.Wave;
using KeyEngine.Assets;
using OpenTK.Audio.OpenAL;
using KeyEngine.Serialization;
using System.Runtime.InteropServices;

namespace KeyEngine.Audio
{
    // TODO: Потестировать работу полной версии NAudio на других платформах кроме Windows (Полная версия нужна для загрузки mp3)
    public class AudioSample : Asset, IDisposable
    {
        public int BufferHandle { get; private set; } = -1;


        private IntPtr dataPointer;
        private bool disposed;

        public override bool AssetLoaded => BufferHandle != -1 && dataPointer != IntPtr.Zero;

        public AudioSample() { }
        public AudioSample(string filePath) => LoadWavFile(filePath);

        /// <summary>
        /// Loads audio file in wav format
        /// </summary>
        /// <param name="filePath">Path to audio file</param>
        public void LoadWavFile(string path)
        {
            using (WaveFileReader waveFileReader = new WaveFileReader(path))
            {
                WaveStream stream = waveFileReader;

                int channels = stream.WaveFormat.Channels;
                int bits = stream.WaveFormat.BitsPerSample;
                int sampleRate = stream.WaveFormat.SampleRate;

                byte[] array;

                if (channels == 1)
                {
                    array = new byte[waveFileReader.Length];
                    waveFileReader.ReadExactly(array);
                }
                else
                {
                    var l = new StereoToMonoProvider16(waveFileReader);
                    array = new byte[waveFileReader.Length / 2];
                    l.Read(array, 0, array.Length);
                    channels = 1;
                }

                GenDataPointer(array);
                GenDataBuffer(channels, bits, sampleRate, array);
            }
        }
        public void LoadMp3File(string path)
        {
            MpegFile mpegFile = new MpegFile(path);
            byte[] buffer = new byte[4096];
            List<byte> allSamples = [];

            int totalRead;
            while ((totalRead = mpegFile.ReadSamplesInt16(buffer, 0, buffer.Length)) > 0)
            {
                allSamples.AddRange(buffer.Take(totalRead));
            }

            byte[] pcmData = [.. allSamples];

            GenDataPointer(pcmData);
            GenDataBuffer(mpegFile.Channels, 16, mpegFile.SampleRate, pcmData);
        }

        private void GenDataBuffer(int channels, int bits, int sampleRate, byte[] array)
        {
            BufferHandle = AL.GenBuffer();

            AL.BufferData(
            BufferHandle
            , AudioManager.GetSoundFormat(channels, bits)
            , dataPointer
            , array.Length
            , sampleRate);
        }

        private void GenDataPointer(byte[] data)
        {
            if (AudioManager.UseUnsafeCode == true)
            {
                unsafe
                {
                    fixed (byte* p = data)
                    {
                        dataPointer = (IntPtr)p;
                    }
                }
            }
            else
            {
                IntPtr result = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, result, data.Length);
                dataPointer = result;
            }
        }

        private void DisposeUnmanaged()
        {
            if (AssetLoaded)
                AL.DeleteBuffer(BufferHandle);

            if (dataPointer != IntPtr.Zero)
                Marshal.FreeHGlobal(dataPointer);
        }

        internal override void LoadAsset(string path)
        {
            LoadWavFile(path);
            AssetPath = path;
        }

        public string? GetAssetPath()
        {
            return AssetPath;
        }

        public void Dispose()
        {
            if (disposed) return;
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
            disposed = true;
        }

        public void LoadAsset(string path, string dataPath)
        {
            throw new NotImplementedException();
        }

        internal override void UnloadAsset()
        {
            Dispose();
        }

        public override SerializeData Serialize()
        {
            return new SerializeData();
        }

        public override void Deserialize(SerializeData data)
        {
            
        }
    }
}
