using KeyEngine.Audio;
using KeyEngine.GUI;
using KeyEngine.Mathematics;
using KeyEngine.Tests;

namespace KeyEngine
{
    public class AudioScene : IScene
    {
        private readonly AssetReference<AudioSample> audio = new AssetReference<AudioSample>("Assets/Audio/Vigilantism.wav");

        public void Load()
        {
            Entity audioListener = ECS.AddEntity("Audio Listener");
            audioListener.AddComponent<AudioListener>();
            audioListener.AddComponent<SpriteRenderer>().Color = new Color32(255, 0, 255);
            audioListener.AddComponent<ListenerRotation>();

            Entity audioSourceEntity = ECS.AddEntity("Audio Source");
            AudioSource audioSource = audioSourceEntity.AddComponent<AudioSource>();
            audioSource.ReferenceDistance = 4.4f;
            audioSource.PanSmoothness = 2.5f;
            audioSource.SetAudioSample(audio.Value);
            audioSource.Play();
            audioSourceEntity.AddComponent<SpriteRenderer>();
            audioSourceEntity.Scale = new Vector2(0.5f, 0.5f);

            Entity batch = ECS.AddEntity("Batch");
            //batch.AddComponent<BatchRendering>();

            //Entity button = ECS.AddEntity();
            //button.AddComponent<SpriteRenderer>();
            //button.Position = new Vector2(10, 0);
            //button.AddComponent<Button>().Init();
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
