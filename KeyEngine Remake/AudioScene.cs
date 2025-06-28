using KeyEngine.Audio;

namespace KeyEngine
{
    public class AudioScene : IScene
    {
        private readonly AssetReference<AudioSample> audio = new AssetReference<AudioSample>("Assets/Audio/Vigilantism.wav");

        public void Load()
        {
            Entity audioListener = ECS.AddEntity("Audio Listener");
            audioListener.AddComponent<AudioListener>();
            audioListener.AddComponent<SpriteRenderer>().Color = new Color(255, 0, 255);
            audioListener.AddComponent<ListenerRotation>();

            Entity audioSourceEntity = ECS.AddEntity("Audio Source");
            AudioSource audioSource = audioSourceEntity.AddComponent<AudioSource>();
            audioSource.ReferenceDistance = 4.4f;
            audioSource.PanSmoothness = 2.5f;
            audioSource.SetAudioSample(audio.Value);
            audioSource.Play();
            audioSourceEntity.AddComponent<SpriteRenderer>();
            audioSourceEntity.Scale = new Vector2(0.5f, 0.5f);
        }

        public void Unload() { }

        private class ListenerRotation : Component
        {
            public float Speed = 3;
            public float Radius = 4.2f;
            private float angle = 0;

            public ListenerRotation(Entity owner) : base(owner) { }

            public override void Update(float deltaTime)
            {
                float x = MathF.Cos(angle) * Radius;
                float y = MathF.Sin(angle) * Radius;

                Owner.Position = new Vector2(x, y);

                angle += Speed * deltaTime;
            }
        }
    }
}
