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
        public static bool UseUnsafeCode { get; set; } = true;

        private static ALContext currentContext;
        private static ALContext nullContext;
        private static ALDevice currentDevice;

        unsafe static AudioManager()
        {
            currentDevice = ALC.OpenDevice(null);

            currentContext = ALC.CreateContext(currentDevice, new ALContextAttributes());
            nullContext = ALC.CreateContext(currentDevice, new ALContextAttributes());
            ALC.MakeContextCurrent(currentContext);

            AL.DistanceModel(ALDistanceModel.None);
        }

        public static void SetPause(bool paused)
        {
            Log.Print(paused);

            if (paused)
            {
                //ALC.CloseDevice(currentDevice);
                ALC.MakeContextCurrent(nullContext);
            }
            else
            {
                ALC.MakeContextCurrent(currentContext);
            }
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
