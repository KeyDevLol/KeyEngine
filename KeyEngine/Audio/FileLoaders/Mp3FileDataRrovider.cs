using NLayer;

namespace KeyEngine.Audio.FileLoaders
{
    public class Mp3FileDataRrovider : IAudioFileDataProvider
    {
        public int Channels { get; private set; }

        public int BitsPerSample { get; private set; }

        public int SampleRate { get; private set; }

        public byte[] PcmData { get; private set; } = null!;

        public Mp3FileDataRrovider(string path)
        {
            Load(path);
        }

        public void Load(string path)
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

            Channels = mpegFile.Channels;
            BitsPerSample = 16;
            SampleRate = mpegFile.SampleRate;
            PcmData = pcmData;
        }
    }
}
