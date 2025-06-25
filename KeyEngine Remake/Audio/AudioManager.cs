using OpenTK.Audio.OpenAL;

namespace KeyEngine.Audio
{
    /// <summary>
    /// Audio manager
    /// </summary>
    public static class AudioManager
    {
        /// <summary>
        /// Faster loading of AudioSamples, but requires the use of unsafe code
        /// </summary>
        public static bool UseUnsafeCode = true;
        private static ALContext currentContext;
        private static ALDevice currentDevice;

        static AudioManager()
        {
            currentDevice = ALC.OpenDevice(null);

            currentContext = ALC.CreateContext(currentDevice, new ALContextAttributes());
            ALC.MakeContextCurrent(currentContext);

            AL.DistanceModel(ALDistanceModel.None);
        }

        /// <summary>
        /// Changes the current audio output source
        /// </summary>
        /// <param name="deviceName"></param>
        //public static void ChangeDevice(string deviceName)
        //{
        //    if (initialized)
        //    {
        //        ALC.DestroyContext(currentContext);
        //        ALC.CloseDevice(currentDevice);
        //    }

        //    currentDevice = ALC.OpenDevice(deviceName);

        //    currentContext = ALC.CreateContext(currentDevice, new ALContextAttributes());
        //    ALC.MakeContextCurrent(currentContext);
        //}

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException();
            }
        }
    }
}
