using NAudio.Wave;

namespace KeyEngine.Audio.FileLoaders
{
    public class WavFileDataProvider : IAudioFileDataProvider
    {
        public int Channels { get; private set; }

        public int BitsPerSample { get; private set; }

        public int SampleRate { get; private set; }

        public byte[] PcmData { get; private set; } = null!;


        public WavFileDataProvider(string path)
        {
            Load(path);
        }

        private void Load(string path)
        {
            using WaveFileReader waveFileReader = new WaveFileReader(path);
            WaveStream stream = waveFileReader;

            Channels = stream.WaveFormat.Channels;
            BitsPerSample = stream.WaveFormat.BitsPerSample;
            SampleRate = stream.WaveFormat.SampleRate;

            byte[] pcmData;

            if (Channels == 1)
            {
                pcmData = new byte[waveFileReader.Length];
                waveFileReader.ReadExactly(pcmData);
            }
            else
            {
                StereoToMonoProvider16 provider = new StereoToMonoProvider16(waveFileReader);
                pcmData = new byte[waveFileReader.Length / 2];
                provider.Read(pcmData, 0, pcmData.Length);
                Channels = 1;
            }

            PcmData = pcmData;
        }
    }
}
