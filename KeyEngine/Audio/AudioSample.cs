using System.Runtime.InteropServices;
using KeyEngine.Audio.FileLoaders;
using KeyEngine.Serialization;
using OpenTK.Audio.OpenAL;
using KeyEngine.Assets;

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
        public AudioSample(string path)
        {
            LoadFile(path);
        }

        public void LoadFile(string path)
        {
            IAudioFileDataProvider provider = Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".wav" => new WavFileDataProvider(path),
                ".mp3" => new Mp3FileDataRrovider(path),
                _ => throw new InvalidDataException($"Audio files with type: {Path.GetExtension(path)} are not supported.")
            };

            GenAudioBuffer(provider);
        }

        private void GenAudioBuffer(IAudioFileDataProvider provider)
        {
            dataPointer = GenDataPointer(provider.PcmData);
            BufferHandle = AL.GenBuffer();

            AL.BufferData(
            BufferHandle
            , AudioManager.GetSoundFormat(provider.Channels, provider.BitsPerSample)
            , dataPointer
            , provider.PcmData.Length
            , provider.SampleRate);
        }

        private static IntPtr GenDataPointer(byte[] pcmData)
        {
            if (AudioManager.UseUnsafeCode == true)
            {
                unsafe
                {
                    fixed (byte* p = pcmData)
                    {
                        return (IntPtr)p;
                    }
                }
            }
            else
            {
                IntPtr result = Marshal.AllocHGlobal(pcmData.Length);
                Marshal.Copy(pcmData, 0, result, pcmData.Length);
                return result;
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
            LoadFile(path);
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

        internal override void UnloadAsset()
        {
            Dispose();
        }

        public override SerializeData Serialize()
        {
            return new SerializeData();
        }

        public override void Deserialize(SerializeData data) { }
    }
}
