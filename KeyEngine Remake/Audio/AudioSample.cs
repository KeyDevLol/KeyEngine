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
                    waveFileReader.Read(array, 0, array.Length);
                }
                else
                {
                    var l = new StereoToMonoProvider16(waveFileReader);
                    array = new byte[waveFileReader.Length / 2];
                    l.Read(array, 0, array.Length);
                    channels = 1;
                }

                pointer = GetDataPointer(array);
                BufferHandle = AL.GenBuffer();

                AL.BufferData(
                BufferHandle
                , AudioManager.GetSoundFormat(channels, bits)
                , pointer
                , array.Length
                , sampleRate);
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
