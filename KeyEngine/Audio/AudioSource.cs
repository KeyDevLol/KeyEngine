using KeyEngine.Assets;
using KeyEngine.Mathematics;
using KeyEngine.Rendering.Gizmos;
using OpenTK.Audio.OpenAL;

namespace KeyEngine.Audio
{
    // TODO: Сделать чтобы при изменении позиции Listener SetCalculatedGain вызывался
    // TODO: Оптимизировать использование AL.Source(SourceHandle, ALSourceb.SourceRelative, false)
    // TODO: Сделать чтобы при отключении компонента звук прерывался
    // TODO: Сделать стриминг проигрывание, чтобы не заносить сразу весь файл в память
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
            set { _volume = value; SetCalculatedGain(); }
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
            set { _maxDistance = value; SetCalculatedGain(); }
        }
        private float _maxDistance = 5;

        public float ReferenceDistance
        {
            get => _referenceDistance;
            set { _referenceDistance = value; SetCalculatedGain(); }
        }
        private float _referenceDistance = 1;

        public float Rolloff
        {
            get => _rolloff;
            set { _rolloff = value; SetCalculatedGain(); }
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
        }

        public override void Update(float deltaTime)
        {
            SetCalculatedGain();
        }

#if ENABLE_EDITOR
        public override void RenderSelectedGizmos()
        {
            GizmosRendering.DrawCircle(Owner.Position, new Vector2(MaxDistance * 2));
            GizmosRendering.DrawCircle(Owner.Position, new Vector2(ReferenceDistance * 2), Color01.Yellow);
        }
#endif

        public float CalculateGain(float distance)
        {
            float volume = Mathf.Clamp01(_volume / 100f);
            float range = MaxDistance - ReferenceDistance;
            if (range <= 0f) return volume;

            distance = Mathf.Clamp(distance, ReferenceDistance, MaxDistance);
            float attenuation = 1f - Rolloff * (distance - ReferenceDistance) / range;

            return volume * Mathf.Clamp01(attenuation);
        }

        private void OnOwnerTransformChanged()
        {
            SetSourcePos();
        }

        public void Play()
        {
            if (AudioSample != null && AudioSample.AssetLoaded)
            {
                AL.SourcePlay(SourceHandle);
            }
        }

        public void SetAudioSample(AudioSample audioSample)
        {
            ArgumentNullException.ThrowIfNull(audioSample);

            if (audioSample.AssetLoaded)
            {
                assetAudioSample.Value = audioSample;
                AL.Source(SourceHandle, ALSourcei.Buffer, audioSample.BufferHandle);
            }
            else
            {
                throw new ArgumentException("AudioSample data is not loaded.");
            }
        }

        private void PitchChanged()
        {
            AL.Source(SourceHandle, ALSourcef.Pitch, _pitch);
        }

        private void LoopingChanged()
        {
            AL.Source(SourceHandle, ALSourceb.Looping, _looping);
        }

        private void PanSmoothnessChanged()
        {
            SetSourcePos();
        }

        private void SetCalculatedGain()
        {
            if (AudioListener.Instance != null && EnableRolloff)
                AL.Source(SourceHandle, ALSourcef.Gain, CalculateGain(Vector2.Distance(Owner.Position, AudioListener.Instance.Owner.Position)));
            else
                AL.Source(SourceHandle, ALSourcef.Gain, _volume / 100);
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

        public override void OnDeleted()
        {
            shouldTaskClose = true;
        }
    }
}
