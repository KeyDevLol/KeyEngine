using KeyEngine.Editor;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;

namespace KeyEngine.Audio
{
    // TODO: Сделать чтобы при изменении позиции Listener SetCalculatedGain вызывался
    // TODO: Оптимизировать использование AL.Source(SourceHandle, ALSourceb.SourceRelative, false)
    public class AudioSource : Component
    {
        public readonly int SourceHandle;

        public AudioSample? AudioSample
        {
            get => assetAudioSample.Value;
            set => assetAudioSample.Value = value;
        }
        private readonly AssetReference<AudioSample> assetAudioSample = new AssetReference<AudioSample>();

        public bool IsPlaying => (ALSourceState)AL.GetSource(SourceHandle, ALGetSourcei.SourceState) == ALSourceState.Playing;
        public bool Looping
        {
            get => _looping;
            set { _looping = value; LoopingChanged(); }
        }
        private bool _looping;
        public float Volume
        {
            get => _volume;
            set { _volume = value; VolumeChanged(); }
        }
        private float _volume = 100;
        public float Pitch
        {
            get => _pitch;
            set { _pitch = value; PitchChanged(); }
        }
        private float _pitch = 1;

        public bool EnablePanning
        {
            get => _enablePanning;
            set { _enablePanning = value; SetSourcePos(); }
        }
        private bool _enablePanning = true;
        public bool EnableRolloff { get; set; } = true;

        public float MaxDistance
        {
            get => _maxDistance;
            set { _maxDistance = value; MaxDistanceChanged(); }
        }
        private float _maxDistance = 5;

        public float ReferenceDistance
        {
            get => _referenceDistance;
            set { _referenceDistance = value; ReferenceDistanceChanged(); }
        }
        private float _referenceDistance = 1;

        public float Rolloff
        {
            get => _rolloff;
            set { _rolloff = value; RolloffChanged(); }
        }
        private float _rolloff = 1;

        public float PanSmoothness
        {
            get => _panSmoothness;
            set { _panSmoothness = value; PanSmoothnessChanged(); }
        }
        private float _panSmoothness = 1;

        private bool shouldTaskClose;

        ~AudioSource()
        {
            shouldTaskClose = true;
        }

        public AudioSource(Entity owner) : base(owner)
        {
            SourceHandle = AL.GenSource();
            AL.Source(SourceHandle, ALSourceb.SourceRelative, false);

            Owner.OnTransformChanged += OnOwnerTransformChanged;
            SetSourcePos();
            SetCalculatedGain();

            Task.Factory.StartNew(() =>
            {
                while (!shouldTaskClose)
                {
                    SetCalculatedGain();
                }
            });
        }

        public float CalculateGain(float distance)
        {
            float volume = _volume / 100f;
            volume = Math.Clamp(volume, 0.0f, 1.0f);
            distance = Math.Clamp(distance, ReferenceDistance, MaxDistance);

            float distanceAttenuation = 1.0f - Rolloff * (distance - ReferenceDistance) / (MaxDistance - ReferenceDistance);
            float gain = volume * Math.Clamp(distanceAttenuation, 0.0f, 1.0f);

            return gain;
        }

        private void OnOwnerTransformChanged()
        {
            SetSourcePos();
            Log.Print("Changed");
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsKeyPressed(KeyCode.Space))
                Play();
        }

        public void Play()
        {
            if (AudioSample != null && AudioSample.Loaded)
            {
                AL.SourcePlay(SourceHandle);
            }
        }

        public void SetAudioSample(AudioSample audioSample)
        {
            ArgumentNullException.ThrowIfNull(audioSample);

            if (audioSample.Loaded)
            {
                assetAudioSample.Value = audioSample;
                AL.Source(SourceHandle, ALSourcei.Buffer, audioSample.BufferHandle);
            }
            else
            {
                throw new Exception("AudioSample data is not loaded.");
            }
        }

        private void VolumeChanged()
        {
            SetCalculatedGain();
        }

        private void PitchChanged()
        {
            AL.Source(SourceHandle, ALSourcef.Pitch, _pitch);
        }

        private void LoopingChanged()
        {
            AL.Source(SourceHandle, ALSourceb.Looping, _looping);
        }

        private void MaxDistanceChanged()
        {
            SetCalculatedGain();
        }

        private void ReferenceDistanceChanged()
        {
            SetCalculatedGain();
        }       
        
        private void RolloffChanged()
        {
            SetCalculatedGain();
        }        

        private void PanSmoothnessChanged()
        {
            SetSourcePos();
        }

        private void SetCalculatedGain()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (AudioListener.Instance != null && EnableRolloff)
                AL.Source(SourceHandle, ALSourcef.Gain, CalculateGain(Vector2.Distance(Owner.Position, AudioListener.Instance.Owner.Position)));
            else
                AL.Source(SourceHandle, ALSourcef.Gain, _volume / 100);
            sw.Stop();
            //Log.Print(sw.Elapsed.TotalMilliseconds);
        }

        private void SetSourcePos()
        {
            if (AudioListener.Instance != null && _enablePanning)
            {
                AL.Source(SourceHandle, ALSourceb.SourceRelative, false);
                AL.Source(SourceHandle, ALSource3f.Position, Owner.Position.X, Owner.Position.Y, PanSmoothness);
            }
            else
            {
                AL.Source(SourceHandle, ALSourceb.SourceRelative, true);
                AL.Source(SourceHandle, ALSource3f.Position, 0, 0, PanSmoothness);
            }
        }

        public override void Deleted()
        {
            shouldTaskClose = true;
        }
    }
}
