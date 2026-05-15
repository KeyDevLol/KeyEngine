namespace KeyEngine.Audio.FileLoaders
{
    public interface IAudioFileDataProvider
    {
        public int Channels { get; }
        public int BitsPerSample { get; }
        public int SampleRate { get; }
        public byte[] PcmData { get; }
    }
}
